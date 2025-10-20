-- Ensure every image starts non-cover
UPDATE [Images] SET [IsCover] = 0;

-- Set a cover image for a given product by URL
-- Replace the URL below with the one you want to be the cover for its product
UPDATE i
SET i.IsCover = 1
FROM Images i
WHERE i.Url = '/images/pants/Elizabeth_Franchie_Pants.jpg';

-- Alternatively, set by Image Id
-- UPDATE Images SET IsCover = 1 WHERE Id = 'PUT-IMAGE-GUID-HERE';

-- Optional: if you want to enforce only one cover per product at SQL level as well,
-- the filtered unique index created by the migration will handle it.
