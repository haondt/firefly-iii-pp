\connect firefly_iii_pp;

CREATE TABLE IF NOT EXISTS "firefly_iii_pp" (
    PrimaryKey TEXT PRIMARY KEY,
    KeyString TEXT NOT NULL,
    Value JSONB NOT NULL
);

CREATE TABLE IF NOT EXISTS "foreignKeys" (
    id SERIAL PRIMARY KEY,
    ForeignKey TEXT,
    KeyString TEXT NOT NULL,
    PrimaryKey TEXT REFERENCES "firefly_iii_pp"(PrimaryKey) ON DELETE CASCADE,
    CONSTRAINT unique_ForeignKey_PrimaryKey UNIQUE (ForeignKey, PrimaryKey)
);

CREATE INDEX IF NOT EXISTS idx_foreign_key
ON "foreignKeys"(ForeignKey);

GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA public TO firefly_iii_pp;
GRANT USAGE, SELECT ON ALL SEQUENCES IN SCHEMA public TO firefly_iii_pp;

CREATE DATABASE testing;
\connect testing;

CREATE TABLE IF NOT EXISTS "firefly_iii_pp" (
    PrimaryKey TEXT PRIMARY KEY,
    KeyString TEXT NOT NULL,
    Value JSONB NOT NULL
);

CREATE TABLE IF NOT EXISTS "foreignKeys" (
    id SERIAL PRIMARY KEY,
    ForeignKey TEXT,
    KeyString TEXT NOT NULL,
    PrimaryKey TEXT REFERENCES "firefly_iii_pp"(PrimaryKey) ON DELETE CASCADE,
    CONSTRAINT unique_ForeignKey_PrimaryKey UNIQUE (ForeignKey, PrimaryKey)
);

CREATE INDEX IF NOT EXISTS idx_foreign_key
ON "foreignKeys"(ForeignKey);

GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA public TO firefly_iii_pp;
GRANT USAGE, SELECT ON ALL SEQUENCES IN SCHEMA public TO firefly_iii_pp;