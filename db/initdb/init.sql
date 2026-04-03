-- init SQL for first-time Postgres container setup
-- This runs only on empty data volume at first initialization.


CREATE SCHEMA IF NOT EXISTS app.poems;

SET search_path TO app.poems;

CREATE TABLE IF NOT EXISTS original (
    id SERIAL PRIMARY KEY,
    title TEXT NOT NULL,
    author TEXT NOT NULL,
    content TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS translations (
    id SERIAL PRIMARY KEY,
    original_id INTEGER NOT NULL,
    language TEXT NOT NULL,
    content TEXT NOT NULL
);

ALTER TABLE translations ADD CONSTRAINT fk_original_id 
    FOREIGN KEY (original_id) REFERENCES original(id)
;

