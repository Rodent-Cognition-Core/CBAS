# Define your item pipelines here
#
# Don't forget to add your pipeline to the ITEM_PIPELINES setting
# See: https://docs.scrapy.org/en/latest/topics/item-pipeline.html


# useful for handling different item types with a single interface

import datetime

class PubscreenScrapyPipeline:

    def process_item(self, item, spider):

        # Extract necessary information from crawler item
        [day, month, year] = item['pubDate'][0].split(' ')[1:4]
        pubdate = datetime.datetime.strptime(day + ' ' + month + ' ' + year, '%d %b %Y').date()
        pubmedid = int([s for s in item['pubIdentifiers'] if 'pmid:' in s][0].split(':')[1])
        pubdoi = [s for s in item['pubIdentifiers'] if 'doi:' in s][0].split(':')[1]
        pubTitle = item['pubTitle'][0]

        if pubdate < spider.cutoffdate:
            return item

        spider.cursor.execute(f"SELECT COUNT(PubmedID) FROM PubmedQueue WHERE PubmedID = {pubmedid}")
        inqueue = spider.cursor.fetchone()[0]

        if inqueue != 0:
            return item

        spider.cursor.execute(f"SELECT COUNT(DOI) FROM Publication WHERE DOI = '{pubdoi}'")
        indb = spider.cursor.fetchone()[0]

        if indb != 0:
            return item

        # print("Adding to queue...")

        today = datetime.date.today()
        #print(pubTitle)
        #print(f"INSERT INTO PubmedQueue (PubmedID, PubDate, QueueDate, DOI, Title) VALUES ({pubmedid}, '{pubdate}', '{today}', '{pubdoi}', '{pubTitle}')")
        spider.cursor.execute(f"INSERT INTO PubmedQueue (PubmedID, PubDate, QueueDate, DOI, Title) VALUES "
                              f"({pubmedid}, '{pubdate}', '{today}', '{pubdoi}', '{pubTitle}')")
        spider.conn.commit()
        spider.is_queue_updated = True

        return item
