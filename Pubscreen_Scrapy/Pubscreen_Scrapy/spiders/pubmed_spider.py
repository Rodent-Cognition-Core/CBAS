import scrapy
from scrapy import signals
from scrapy import Spider
import pyodbc
import datetime
import smtplib, ssl
from email.mime.text import MIMEText
from ..items import PubmedItem
import os

# CALL WITH THE FOLLOWING COMMAND: scrapy crawl pubmed_spider --logfile log.txt --loglevel ERROR
# Add ' -o rss.jl ' to receive the complete list of parsed papers


class PubmedSpider(Spider):

    name = "pubmed_spider"

    def __init__(self):
        # Connect to pubscreen database
        self.conn = pyodbc.connect('Driver={SQL Server};'
                                   'Server=.\\SQLEXPRESS;'
                                   'Database=Pubscreen_NEW;'
                                   'Trusted_Connection=True')
        self.cursor = self.conn.cursor()

        # Determine the cutoff date by getting the last date the queue was updated
        self.cutoffdate = datetime.date
        self.cursor.execute("SELECT MAX(Date) FROM DateQueueUpdated WHERE IsError = 0")
        cutoff = self.cursor.fetchone()[0]
        if cutoff:
            self.cutoffdate = datetime.datetime.strptime(cutoff, "%Y-%m-%d").date()
        else:  # if the queue has not yet been updated, set cutoff to beginning of 2021
            self.cutoffdate = datetime.date(2021, 1, 1)

        # Initialize output states
        self.is_queue_updated = False;
        self.is_error = False;

        # Clear log file
        open("log.txt", "w").close()

    def start_requests(self):
        urls = [
            'https://pubmed.ncbi.nlm.nih.gov/rss/search/1vuK0yqTJNPs5pN9O2pJRjRRZUCz7vEjCJ6DCeUCYqOqb2tkAF/?limit=20&utm_\
            campaign=pubmed-2&fc=20210210200323',  # touchscreen rat
            'https://pubmed.ncbi.nlm.nih.gov/rss/search/16C1W1gAKSbhWNGv9MxE4IJufma1tsxxlyC9Dpf3M1gmTJ-cFT/?limit=20&utm_\
            campaign=pubmed-2&fc=20210210202215',   # touchscreen mouse
            'https://pubmed.ncbi.nlm.nih.gov/rss/search/1reoklHRwDJrydqRh5h80R4vV9yit5R40Ar2SIZX5iFuukhozn/?limit=20&utm_\
            campaign=pubmed-2&fc=20210210202308',   # touchscreen rodent
            'https://pubmed.ncbi.nlm.nih.gov/rss/search/1zKxzw-OuqBQ1DL8H4Yc8IzZUoJiZt8rmqgrVDu1Ho61BLfgHW/?limit=8&utm_\
            campaign=pubmed-2&fc=20210210203655',   # touch screen rat ***limit is 8 due to pubmed internal server error***
            'https://pubmed.ncbi.nlm.nih.gov/rss/search/12__a65sE8_iVP0NFNYc0R-4tU-BOHJeJnuubPOqmenZ186M_b/?limit=20&utm_\
            campaign=pubmed-2&fc=20210210202601',   # touch screen mouse
            'https://pubmed.ncbi.nlm.nih.gov/rss/search/1jq74NZspErFZpXOJ1CnbHE0Kf9EU5qBhFh3gMbeDIhywtZQYn/?limit=20&utm_\
            campaign=pubmed-2&fc=20210210202657',   # touch screen rodent
            'https://pubmed.ncbi.nlm.nih.gov/rss/search/1ZIrvZUsS726d2sp95FEAx6F7CjJ2ZhBElEDTHmOzKHT7UMgR_/?limit=20&utm_\
            campaign=pubmed-2&fc=20210210202801',   # touch-screen rat
            'https://pubmed.ncbi.nlm.nih.gov/rss/search/1VONWHHyVP6u8drHxyLUmhamHarTrlbOapnujtV8ZvROdZ-Qen/?limit=20&utm_\
            campaign=pubmed-2&fc=20210210202850',   # touch-screen mouse
            'https://pubmed.ncbi.nlm.nih.gov/rss/search/1dGikNNbKkSmJICqRi0OyMPUs3iyLDfpd_yEovg1yWlwxLrkYv/?limit=20&utm_\
            campaign=pubmed-2&fc=20210210202921',   # touch-screen rodent
        ]
        for url in urls:
            yield scrapy.Request(url=url, callback=self.parse, errback=self.error_log)

    @classmethod
    def from_crawler(cls, crawler, *args, **kwargs):
        spider = super(PubmedSpider, cls).from_crawler(crawler, *args, **kwargs)
        crawler.signals.connect(spider.spider_closed, signal=signals.spider_closed)
        return spider

    def parse(self, response):

        pubmeditem = PubmedItem()

        for item in response.css('item'):
            item.remove_namespaces()
            pubmeditem['pubTitle'] = item.css('title::text').get(),
            pubmeditem['pubDate'] = item.css('pubDate::text').get(),
            pubmeditem['pubLink'] = item.css('link::text').get()
            pubmeditem['pubIdentifiers'] = item.css('identifier::text').getall()
            yield pubmeditem

    def error_log(self, failure):
        self.logger.error(repr(failure))
        self.logger.error(f"Error on {failure.request.url}")

    def spider_closed(self, spider):

        # Check for error
        if os.path.getsize("log.txt") > 0:
            self.is_error = True

        # Update the date queue was updated
        today = datetime.date.today()
        self.cursor.execute(f"INSERT INTO DateQueueUpdated (Date, IsError) VALUES ('{today}', '{self.is_error}')")
        self.conn.commit()

        # If an email needs to be sent, set up email server
        if self.is_error or self.is_queue_updated:
            port = 587
            context = ssl.create_default_context()
            sender_email = "mousebyt@uwo.ca"
            with smtplib.SMTP("smtp.office365.com", port) as server:
                server.starttls(context=context)
                server.login(sender_email, "")

                # If there was an error, send email to Eric
                if self.is_error:
                    receiver_email = "ejiang6@uwo.ca"
                    msg = MIMEText(open("log.txt").read())
                    msg['Subject'] = "ERROR IN PUBMED_SPIDER"
                    msg['From'] = sender_email
                    msg['To'] = receiver_email
                    server.sendmail(sender_email, receiver_email, msg.as_string())

                # If queue was updated, send email to Julie
                if self.is_queue_updated:
                    receiver_email = "ejiang6@uwo.ca"
                    msg = MIMEText(open("julie_msg.txt").read())
                    msg['Subject'] = "Pubscreen Queue Updated"
                    msg['From'] = sender_email
                    msg['To'] = receiver_email
                    server.sendmail(sender_email, receiver_email, msg.as_string())
