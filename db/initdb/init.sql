-- init SQL for first-time Postgres container setup
-- This runs only on empty data volume at first initialization.
ALTER DATABASE poetryglobal OWNER TO poetryglobal;

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
    author TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS poem_versions (
    poem_metadata_id INT NOT NULL,
    language_id INT NOT NULL,
    is_original BOOLEAN,
    version_text TEXT NOT NULL,
    PRIMARY KEY (poem_metadata_id, language_id)
);

ALTER TABLE poem_metadata ADD CONSTRAINT unique_title_author UNIQUE (title, author);

ALTER TABLE poem_versions ADD CONSTRAINT fk_poem_metadata
    FOREIGN KEY (poem_metadata_id) REFERENCES poem_metadata(id) 
;

ALTER TABLE poem_versions ADD CONSTRAINT fk_language
    FOREIGN KEY (language_id) REFERENCES languages(id)
;

/** Making sure there is only one original version per poem */
CREATE UNIQUE INDEX IF NOT EXISTS idx_one_original_per_poem 
ON poem_versions (poem_metadata_id)
WHERE is_original = true;


/** For trigram-based searches of `poems` by `author` and `title`, I set up trigram indexes on these columns. */ 
CREATE INDEX IF NOT EXISTS idx_poems_author ON poem_metadata USING GIN (author public.gin_trgm_ops);
CREATE INDEX IF NOT EXISTS idx_poems_title ON poem_metadata USING GIN (title public.gin_trgm_ops);

/** Setting pg_trgm parameters for reasonably lax similarity thresholds */
ALTER DATABASE poetryglobal SET pg_trgm.strict_word_similarity_threshold TO 0.3;
ALTER DATABASE poetryglobal SET pg_trgm.word_similarity_threshold TO 0.3;
ALTER DATABASE poetryglobal SET pg_trgm.similarity_threshold TO 0.2;

/** Initialize language codes */
INSERT INTO languages (code) VALUES
    ('en'),
    ('es'),
    ('fr'),
    ('de'),
    ('it'),
    ('pt'),
    ('ru'),
    ('zh'),
    ('ja'),
    ('ko'),
    ('ar'),
    ('hi'),
    ('pl'),
    ('nl'),
    ('sv'),
    ('no'),
    ('da'),
    ('fi'),
    ('tr'),
    ('cs'),
    ('sk'),
    ('uk')
;

RESET search_path;