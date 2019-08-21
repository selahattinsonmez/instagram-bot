# instagram-bot
It brings locations of posts of an instagram account.If you want another informations from instagram you should examine page source 
of the URL that includes informations that you need and change related functions to find informations from source.

You should download chromedriver from https://chromedriver.chromium.org.
You should download HtmlAgilityPack ve Selenium into your applications NuGet package.

This program opens given instagram URL with Selenium in Chrome and scrolls the bar to the end of the page.While it is scrolling it takes 
URLs of posts.Then there two functions to bring URLs of the locations that includes in the posts;
1. With HtmlAgilityPack: This function brings data faster but maybe sometimes it gives error.Cause it loads the page source without login.
2. With Selenium: This function is very slow.But it definitly works cause of it works it logged in page.
Then getLocationInformation method extracts information that you need from page source.
