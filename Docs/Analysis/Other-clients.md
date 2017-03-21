# Other clients

An application that allows humans to play against an AI is called a **client** and it connects either to an **online server** or to a **Go-playing program** (an AI).

The Go-playing programs appeared around 1970 and so did clients, but the first public online server was Pandanet (then called simply _Internet Go Server_) in 1992 (see our documentation on IGS).

There are six well-known online servers (and many smaller ones) and dozens of clients. We cannot look at all but we did have a look at the most popular ones.

We have downloaded and used 20 different clients and compared their features. The table that lists differences between 19 of these is attached to this document as an Excel file. Martin Dvořák, then a member of our team, contributed to this effort.

Two notable programs are not included in our analysis because they either didn't exist at that time (Ancient Go) or we weren't aware of them (Go Universe). 

Go Universe is notable because it's a Google Chrome plugin that acts as a client to KGS, using the same API as we do. It's Go Universe that convinced us making a KGS client is possible. Until Go Universe began in 2016, one could only use the official client to connect to KGS.

Ancient Go is notable because it feels more like a video game than an application as Go clients typically are, and is also available on Steam. Ancient Go's developer gave us some advice on our project.

A common thread linking most clients is that they are single-purpose. Few clients allow both local and online play. Few applications offer both tsumego solving and play. No application allows you to play on multiple servers. This makes sense - developers focus on a smaller domain and and the users may use the application that's best for that particular part of the game - but having a single application for everything related to Go is also useful, enables sharing code among the features and provides a single experience.

That is what we have decided to do. We offer few features that are not present in at least one other Go client (we are unique in having a singleplayer rewards system and extensive help pages) but we include features from many clients and make them work together.