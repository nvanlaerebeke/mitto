.. image:: ./docs/images/logo.jpg

Mitto is a scalable Client/Server framework with a Request/Response model written in C#.

The aim is to provide a simple framework that is easy to start with and can be scaled up when needed and have each component as interchangable as possible so the user has the flexability to customerize each component for its project


For a more mature option that's maintained a good alternative `RocketLib <http://rockframework.org>`_ is a good option.

Introduction
============

Mitto was created to solve the problem of communication between a Client/Server and build on a few simple concepts:

- reusable: easy to set up and use in multiple projects & languages
- scalable: both the client as server should be able to scale horizontally 
- moduler: have the ability switch out technologies, Websockets/Json messages, message queing


Mitto is usable on any scale from small projects that require only a few connections and messages to projects that can scale to thousands of clients.

Each component can also be switched out for other technologies and languages.

Table of Contents
-----------------

.. toctree::

   quickstart
   gettingstarted
   connection
   messaging
     messageconverter
     messageprocessor
   queue
   samples