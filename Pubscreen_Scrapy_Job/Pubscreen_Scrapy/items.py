# Define here the models for your scraped items
#
# See documentation in:
# https://docs.scrapy.org/en/latest/topics/items.html

import scrapy


class PubmedItem(scrapy.Item):
    # define the fields for your item here like:
    pubTitle = scrapy.Field()
    pubDate = scrapy.Field()
    pubLink = scrapy.Field()
    pubIdentifiers = scrapy.Field()
    pass
