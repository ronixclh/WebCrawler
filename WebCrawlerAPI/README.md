# WebCrawler API

## Overview

**WebCrawler API** is a .NET Core web application that scrapes news entries from [Hacker News](https://news.ycombinator.com/) and provides a REST API to filter these entries based on the number of words in the title and their popularity. 

FIrst 30 entries are scrapped and logged in the console.

The API offers two endpoints:

1. `/api/news/morethan5words` - Returns news entries with more than 5 words in the title, sorted by the number of comments in descending order.
2. `/api/news/lessthanorequal5words` - Returns news entries with 5 or fewer words in the title, sorted by points in descending order.

The application logs requests to a local SQLite database named `WebCrawler.db`, which is stored at WebCrawler\WebCrawlerAPI\bin\Debug\net8.0 or in app/ directory in docker files. 

---

## Features

- Scrape the first 30 entries from Hacker News.
- Filter entries based on word count in the title.
- Return the filtered entries sorted by the number of comments or points.
- Log API requests with timestamps and the applied filter to an SQLite database.
- Dockerized for easy deployment and local development.
- **Continuous Integration**: GitHub Actions are set up to automatically run unit tests on each commit or pull request.

---

## Prerequisites

To run the project, ensure you have the following installed:

- .NET SDK 8.0 or later
- Docker (for containerized setup)

---

## Running the Application Locally

### Step 1: Clone the Repository

````bash
git clone https://github.com/your-repository/webcrawler-api.git
````

### Step 2: Install Dependencies

````bash
cd WebCrawlerAPI
dotnet restore
````

### Step 3: Run the Application

By default, it runs on https://localhost:7089. You can access the Swagger UI for API testing at:

````bash
https://localhost:7089/index.html
````

## Running the Application Locally

### Step 1: Build and Run Docker Containers

````bash
docker-compose up --build
````

### Step 2: Access the API

````bash
http://localhost:8080/index.html
````



