USE Africa;
GO
-- Seed Articles (Africa)
INSERT INTO [Articles] ([ArticleId], [Name], [Content], [Timestamp]) VALUES
('a1b2c3d4-0001-0000-0000-000000000003', '[Africa] The Rise of Renewable Energy', 'Africa''s vast solar potential is being harnessed through large-scale projects in the Sahara and Sub-Saharan regions. Off-grid solar solutions are bringing electricity to millions of rural communities for the first time.', '2026-01-05 08:00:00'),
('a1b2c3d4-0002-0000-0000-000000000003', '[Africa] Advances in Artificial Intelligence', 'African AI startups are developing solutions tailored to local challenges in agriculture, healthcare, and finance. Hubs in Nairobi, Lagos, and Cape Town are nurturing the next generation of AI innovators.', '2026-01-12 09:30:00'),
('a1b2c3d4-0003-0000-0000-000000000003', '[Africa] Space Tourism Takes Off', 'African nations are investing in space programmes, with Kenya and South Africa expanding their satellite capabilities. The continent aims to leverage space technology for weather forecasting and land management.', '2026-01-18 10:15:00'),
('a1b2c3d4-0004-0000-0000-000000000003', '[Africa] The Future of Remote Work', 'The rise of remote work is creating new opportunities for Africa''s young, tech-savvy population. Digital nomad visas and co-working spaces are proliferating in cities like Kigali and Accra.', '2026-01-25 11:00:00'),
('a1b2c3d4-0005-0000-0000-000000000003', '[Africa] Breakthroughs in Cancer Research', 'African research institutions are partnering with global organisations to address the rising burden of cancer on the continent. New mobile screening units are reaching underserved populations in rural areas.', '2026-02-02 08:45:00'),
('a1b2c3d4-0006-0000-0000-000000000003', '[Africa] African Food Security Challenges', 'Prolonged droughts and conflict continue to threaten food security across parts of Africa. Drought-resistant crop varieties and precision agriculture are being deployed to boost resilience and yields.', '2026-02-10 13:00:00'),
('a1b2c3d4-0007-0000-0000-000000000003', '[Africa] Quantum Computing Milestones', 'African universities are beginning to engage with quantum computing research, with partnerships forming between institutions in South Africa and leading global quantum labs to build local expertise.', '2026-02-17 14:30:00'),
('a1b2c3d4-0008-0000-0000-000000000003', '[Africa] Mental Health Awareness in the Digital Age', 'Stigma around mental health is slowly decreasing in Africa, aided by social media campaigns and digital therapy platforms. Community-based approaches are proving effective in reaching those in need.', '2026-02-24 09:00:00'),
('a1b2c3d4-0009-0000-0000-000000000003', '[Africa] Electric Vehicles Dominate Auto Market', 'Electric motorcycles and three-wheelers are transforming urban mobility across African cities. Startups in Rwanda and Nigeria are building EV charging networks to support the growing fleet.', '2026-03-03 10:00:00'),
('a1b2c3d4-0010-0000-0000-000000000003', '[Africa] Cybersecurity in a Connected World', 'Rapid digital adoption across Africa has made cybersecurity a critical priority. Governments and private sector organisations are investing in capacity building and continent-wide incident response frameworks.', '2026-03-09 11:30:00');

INSERT INTO [ArticleAuthors] ([Id], [ArticleId], [AuthorId]) VALUES
-- Article 1
    (NEWID(),'a1b2c3d4-0001-0000-0000-000000000003','c1d2e3f4-0001-0000-0000-000000000001'),
    (NEWID(),'a1b2c3d4-0001-0000-0000-000000000003','c1d2e3f4-0002-0000-0000-000000000002'),
    (NEWID(),'a1b2c3d4-0001-0000-0000-000000000003','c1d2e3f4-0003-0000-0000-000000000003'),

-- Article 2
    (NEWID(),'a1b2c3d4-0002-0000-0000-000000000003','c1d2e3f4-0002-0000-0000-000000000002'),
    (NEWID(),'a1b2c3d4-0002-0000-0000-000000000003','c1d2e3f4-0003-0000-0000-000000000003'),
    (NEWID(),'a1b2c3d4-0002-0000-0000-000000000003','c1d2e3f4-0004-0000-0000-000000000004'),

-- Article 3
    (NEWID(),'a1b2c3d4-0003-0000-0000-000000000003','c1d2e3f4-0003-0000-0000-000000000003'),
    (NEWID(),'a1b2c3d4-0003-0000-0000-000000000003','c1d2e3f4-0004-0000-0000-000000000004'),
    (NEWID(),'a1b2c3d4-0003-0000-0000-000000000003','c1d2e3f4-0005-0000-0000-000000000005'),

-- Article 4
    (NEWID(),'a1b2c3d4-0004-0000-0000-000000000003','c1d2e3f4-0001-0000-0000-000000000001'),
    (NEWID(),'a1b2c3d4-0004-0000-0000-000000000003','c1d2e3f4-0004-0000-0000-000000000004'),
    (NEWID(),'a1b2c3d4-0004-0000-0000-000000000003','c1d2e3f4-0005-0000-0000-000000000005'),

-- Article 5
    (NEWID(),'a1b2c3d4-0005-0000-0000-000000000003','c1d2e3f4-0004-0000-0000-000000000004'),
    (NEWID(),'a1b2c3d4-0005-0000-0000-000000000003','c1d2e3f4-0005-0000-0000-000000000005'),
    (NEWID(),'a1b2c3d4-0005-0000-0000-000000000003','c1d2e3f4-0001-0000-0000-000000000001'),

-- Article 6
    (NEWID(),'a1b2c3d4-0006-0000-0000-000000000003','c1d2e3f4-0005-0000-0000-000000000005'),
    (NEWID(),'a1b2c3d4-0006-0000-0000-000000000003','c1d2e3f4-0001-0000-0000-000000000001'),
    (NEWID(),'a1b2c3d4-0006-0000-0000-000000000003','c1d2e3f4-0002-0000-0000-000000000002'),

-- Article 7
    (NEWID(),'a1b2c3d4-0007-0000-0000-000000000003','c1d2e3f4-0002-0000-0000-000000000002'),
    (NEWID(),'a1b2c3d4-0007-0000-0000-000000000003','c1d2e3f4-0003-0000-0000-000000000003'),
    (NEWID(),'a1b2c3d4-0007-0000-0000-000000000003','c1d2e3f4-0004-0000-0000-000000000004'),

-- Article 8
    (NEWID(),'a1b2c3d4-0008-0000-0000-000000000003','c1d2e3f4-0005-0000-0000-000000000005'),
    (NEWID(),'a1b2c3d4-0008-0000-0000-000000000003','c1d2e3f4-0001-0000-0000-000000000001'),
    (NEWID(),'a1b2c3d4-0008-0000-0000-000000000003','c1d2e3f4-0002-0000-0000-000000000002'),

-- Article 9
    (NEWID(),'a1b2c3d4-0009-0000-0000-000000000003','c1d2e3f4-0003-0000-0000-000000000003'),
    (NEWID(),'a1b2c3d4-0009-0000-0000-000000000003','c1d2e3f4-0002-0000-0000-000000000002'),
    (NEWID(),'a1b2c3d4-0009-0000-0000-000000000003','c1d2e3f4-0004-0000-0000-000000000004'),

-- Article 10
    (NEWID(),'a1b2c3d4-0010-0000-0000-000000000003','c1d2e3f4-0004-0000-0000-000000000004'),
    (NEWID(),'a1b2c3d4-0010-0000-0000-000000000003','c1d2e3f4-0001-0000-0000-000000000001'),
    (NEWID(),'a1b2c3d4-0010-0000-0000-000000000003','c1d2e3f4-0005-0000-0000-000000000005');

