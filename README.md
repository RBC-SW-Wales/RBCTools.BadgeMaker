RBC Badge Maker
===============

This project is a work in progress.

Very short project description:
-------------------------------

We need to build a small application that department Overseers can use to
produce RBC Badges. They'll print off the badges and distribute to each of 
their volunteers before a new RBC project begins. These badges are then 
used on site to identify volunteers and provide training information. 

Some technical details:
-----------------------

I have set up a version control repository to share source code. Actually, 
this README is the first thing I've added to the repository, so you are 
looking at this right now. We are using 'git' for this (and github.com, 
which you may be on right now). If you are unfamiliar with how this works, 
see http://try.github.io/. If you want help with code, you will need to 
sign up to Github and let me know your username.

The application itself will be written in C# on .NET and build for Windows. 
I plan on re-using some of the code from another application that we build 
last year (the RBC Console), to allow us to connect to the volunteers 
database and query the data needed for badges.

At first, we will try and get a working version with no GUI (like the RBC 
Console), but once the core functionality is in place we will need to build 
a GUI also so that it is usable for department Overseers and their Assistants.

The application will produce the badges as a PDF file. I plan on using an open 
source .NET library called PdfSharp for this (see www.pdfsharp.com for more info). 
I have used this successfully for producing print quality PDF files in the past.

More details may follow...
