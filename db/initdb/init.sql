-- init SQL for first-time Postgres container setup
-- This runs only on empty data volume at first initialization.
CREATE EXTENSION IF NOT EXISTS "pg_trgm";

CREATE SCHEMA IF NOT EXISTS app;

SET search_path TO app;

CREATE TABLE IF NOT EXISTS languages (
    id SERIAL PRIMARY KEY,
    code TEXT NOT NULL UNIQUE
);


CREATE TABLE IF NOT EXISTS poem_metadata (
    id SERIAL PRIMARY KEY,
    title TEXT NOT NULL,
    author TEXT NOT NULL,
);

ALTER TABLE poem_metadata ADD CONSTRAINT unique_title_author UNIQUE (title, author);


CREATE TABLE IF NOT EXISTS poem_versions (
    poem_metadata_id INT NOT NULL,
    language_id INT NOT NULL,
    is_original BOOLEAN,
    content TEXT NOT NULL,
    PRIMARY KEY (poem_metadata_id, language_id)
);


ALTER TABLE poem_versions ADD CONSTRAINT fk_poem_metadata
    FOREIGN KEY (poem_metadata_id) REFERENCES poem_metadata(id) 
;

ALTER TABLE poem_versions ADD CONSTRAINT fk_language
    FOREIGN KEY (language_id) REFERENCES languages(id)
;

/** Making sure there is only one original version per poem */
ALTER TABLE poem_versions ADD CONSTRAINT one_original_per_poem 
    UNIQUE (poem_metadata_id) WHERE is_original = true
;

/** 
Languages are searched solely by ID equality. I set a hash index on `languages.id`, 
as it provides O(1) lookups for such searches. The languages table is practically read-only,
which is why the cost of maintaining the hash index is negligible.

The advantage is yet to be verified, however, as B-tree indexes have superior optimizer support 
and scale fairly well despite the higher time complexity.
*/
CREATE INDEX IF NOT EXISTS idx_languages_id ON languages USING HASH (language_id);


/** For keyword-based searches of `poems` by `author` and `title`, I set up trigram indexes on these columns. */ 
CREATE INDEX IF NOT EXISTS idx_poems_author ON poems USING GIN (author gin_trgm_ops);
CREATE INDEX IF NOT EXISTS idx_poems_title ON poems USING GIN (title gin_trgm_ops);



RESET search_path TO default;