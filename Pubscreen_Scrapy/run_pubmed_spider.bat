 call C:\ProgramData\Anaconda3\Scripts\activate.bat 
 call conda activate Pubscreen_Scrapy
 call scrapy crawl pubmed_spider --logfile log.txt --loglevel ERROR -o rss.jl
 