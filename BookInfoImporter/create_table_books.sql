CREATE TABLE [books] (
    [book_id]             INT            NOT NULL,
    [title]              VARCHAR (255)  NOT NULL,
    [authors]            VARCHAR (2048) NOT NULL,
    [average_rating]     FLOAT (53)     NOT NULL,
    [isbn]               VARCHAR (255)  NOT NULL,
    [isbn13]             VARCHAR (255)  NOT NULL,
    [language_code]      VARCHAR (8)    NOT NULL,
    [num_pages]          INT            NOT NULL,
    [ratings_count]      INT            NOT NULL,
    [text_reviews_count] INT            NOT NULL,
    [publication_date]   DATE           NOT NULL,
    [publisher]          VARCHAR (255)  NOT NULL,
    PRIMARY KEY CLUSTERED ([book_id] ASC)
);