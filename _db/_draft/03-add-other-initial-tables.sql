DO $$
BEGIN

-- activity
IF NOT EXISTS (SELECT 1 FROM pg_tables WHERE schemaname = 'public' AND tablename = 'activity') THEN
CREATE TABLE public.activity (
    activity_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    category_id UUID NULL,
    job_id UUID NULL,
    start_time TIMESTAMP NOT NULL,
    end_time TIMESTAMP NOT NULL,
    name TEXT NOT NULL,
    description TEXT,
    created_at TIMESTAMP NOT NULL DEFAULT NOW(),
    modified_at TIMESTAMP NOT NULL DEFAULT NOW(),
    user_id UUID NOT NULL,
    concurrency_stamp UUID NOT NULL DEFAULT gen_random_uuid(),
    status INTEGER NOT NULL
);
END IF;

-- media
IF NOT EXISTS (SELECT 1 FROM pg_tables WHERE schemaname = 'public' AND tablename = 'media') THEN
CREATE TABLE public.media (
    media_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    activity_id UUID NOT NULL,
    original_filename VARCHAR NOT NULL,
    extension VARCHAR NOT NULL,
    mime_type VARCHAR NOT NULL,
    size BIGINT NOT NULL,
    storage_path VARCHAR NOT NULL,
    hash VARCHAR NOT NULL,
    metadata TEXT,
    status INTEGER NOT NULL,
    created_at TIMESTAMP NOT NULL DEFAULT NOW(),
    modified_at TIMESTAMP NOT NULL DEFAULT NOW(),
    user_id UUID NOT NULL,
    concurrency_stamp UUID NOT NULL DEFAULT gen_random_uuid()
);
END IF;

-- category
IF NOT EXISTS (SELECT 1 FROM pg_tables WHERE schemaname = 'public' AND tablename = 'category') THEN
CREATE TABLE public.category (
     category_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
     name VARCHAR NOT NULL,
     description TEXT,
     created_at TIMESTAMP NOT NULL DEFAULT NOW(),
     modified_at TIMESTAMP NOT NULL DEFAULT NOW(),
     user_id UUID NOT NULL,
     concurrency_stamp UUID NOT NULL DEFAULT gen_random_uuid(),
     status INTEGER NOT NULL
);
END IF;

-- job
IF NOT EXISTS (SELECT 1 FROM pg_tables WHERE schemaname = 'public' AND tablename = 'job') THEN
CREATE TABLE public.job (
    job_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    category_id UUID NOT NULL,
    name VARCHAR NOT NULL,
    description TEXT,
    data JSONB,
    created_at TIMESTAMP NOT NULL DEFAULT NOW(),
    modified_at TIMESTAMP NOT NULL DEFAULT NOW(),
    user_id UUID NOT NULL,
    concurrency_stamp UUID NOT NULL DEFAULT gen_random_uuid(),
    status INTEGER NOT NULL
);
END IF;

-- Activity FKs
ALTER TABLE public.activity
    ADD CONSTRAINT fk_activity_category
        FOREIGN KEY (category_id)
            REFERENCES public.category(category_id)
            ON DELETE SET NULL ON UPDATE CASCADE;

ALTER TABLE public.activity
    ADD CONSTRAINT fk_activity_job
        FOREIGN KEY (job_id)
            REFERENCES public.job(job_id)
            ON DELETE SET NULL ON UPDATE CASCADE;

END $$;
