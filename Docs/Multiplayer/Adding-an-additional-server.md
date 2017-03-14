# Adding an additional server

_Relevant requirement:_

*   _8.2\. It will not be difficult to add an additional online server later on._

We designed omegaGo to allow for easily adding support for additional Internet servers. Despite that, however, it remains a significant challenge to do so. This is, unfortunately, inevitable, because there is very little similarity among Go servers and the API of these servers, if public at all, tend to have poor documentation.

To add support for an additional server, you should:

*   Addd