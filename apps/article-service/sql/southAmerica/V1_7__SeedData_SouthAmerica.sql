
USE SouthAmerica;
GO
-- Seed Articles (South America)
INSERT INTO [Articles] ([ArticleId], [Name], [Content], [Timestamp]) VALUES
('a1b2c3d4-0001-0000-0000-000000000006', '[South America] The Rise of Renewable Energy', 'South America is leveraging its abundant natural resources to build one of the world''s cleanest energy mixes. Brazil leads in hydroelectric power, while Chile and Colombia are rapidly expanding solar and wind capacity.', '2026-01-05 08:00:00'),
('a1b2c3d4-0002-0000-0000-000000000006', '[South America] Advances in Artificial Intelligence', 'Brazil and Argentina are emerging as Latin America''s AI powerhouses. Tech ecosystems in São Paulo and Buenos Aires are attracting investment and producing AI solutions for agriculture, fintech, and public services.', '2026-01-12 09:30:00'),
('a1b2c3d4-0003-0000-0000-000000000006', '[South America] Space Tourism Takes Off', 'South American nations are investing in space capabilities to support satellite communications, environmental monitoring, and disaster response. Brazil''s Alcântara Launch Center positions the region for commercial launches.', '2026-01-18 10:15:00'),
('a1b2c3d4-0004-0000-0000-000000000006', '[South America] The Future of Remote Work', 'Remote work is reshaping South American cities, with digital infrastructure improvements enabling millions to work from home. Countries like Uruguay and Colombia are positioning themselves as digital nomad destinations.', '2026-01-25 11:00:00'),
('a1b2c3d4-0005-0000-0000-000000000006', '[South America] Breakthroughs in Cancer Research', 'South American oncology researchers are making progress in treating cancers prevalent in the region, including stomach and cervical cancer. Partnerships with international institutions are bringing cutting-edge therapies to local populations.', '2026-02-02 08:45:00'),
('a1b2c3d4-0006-0000-0000-000000000006', '[South America] South American Food Security Challenges', 'South America feeds much of the world, but deforestation, drought, and inequality threaten its own food security. Regenerative agriculture and agroforestry practices are gaining traction as sustainable alternatives.', '2026-02-10 13:00:00'),
('a1b2c3d4-0007-0000-0000-000000000006', '[South America] Quantum Computing Milestones', 'Universities in Brazil and Argentina are establishing quantum computing research programmes. Regional governments are beginning to recognise quantum technology as a strategic priority for long-term economic competitiveness.', '2026-02-17 14:30:00'),
('a1b2c3d4-0008-0000-0000-000000000006', '[South America] Mental Health Awareness in the Digital Age', 'Mental health conversations are becoming more open across South America, driven by youth-led social media campaigns. Brazil has significantly expanded public mental health services and digital therapy access in recent years.', '2026-02-24 09:00:00'),
('a1b2c3d4-0009-0000-0000-000000000006', '[South America] Electric Vehicles Dominate Auto Market', 'Electric vehicle adoption is growing in South America, led by Chile and Colombia. Expansion of public charging networks and government subsidies are making EVs more accessible to middle-class consumers across the region.', '2026-03-03 10:00:00'),
('a1b2c3d4-0010-0000-0000-000000000006', '[South America] Cybersecurity in a Connected World', 'South America faces increasing cyber threats targeting financial institutions and government systems. Nations are collaborating on regional cybersecurity frameworks and investing in local talent to defend critical infrastructure.', '2026-03-09 11:30:00');

-- Seed ArticleAuthors (South America)
INSERT INTO [ArticleAuthors] ([Id], [ArticleId], [AuthorId]) VALUES
('b1c2d3e4-0001-0000-0000-000000000006', 'a1b2c3d4-0001-0000-0000-000000000006', 'c1d2e3f4-0001-0000-0000-000000000001'),
('b1c2d3e4-0002-0000-0000-000000000006', 'a1b2c3d4-0002-0000-0000-000000000006', 'c1d2e3f4-0002-0000-0000-000000000002'),
('b1c2d3e4-0003-0000-0000-000000000006', 'a1b2c3d4-0003-0000-0000-000000000006', 'c1d2e3f4-0003-0000-0000-000000000003'),
('b1c2d3e4-0004-0000-0000-000000000006', 'a1b2c3d4-0004-0000-0000-000000000006', 'c1d2e3f4-0001-0000-0000-000000000001'),
('b1c2d3e4-0005-0000-0000-000000000006', 'a1b2c3d4-0005-0000-0000-000000000006', 'c1d2e3f4-0004-0000-0000-000000000004'),
('b1c2d3e4-0006-0000-0000-000000000006', 'a1b2c3d4-0006-0000-0000-000000000006', 'c1d2e3f4-0005-0000-0000-000000000005'),
('b1c2d3e4-0007-0000-0000-000000000006', 'a1b2c3d4-0007-0000-0000-000000000006', 'c1d2e3f4-0002-0000-0000-000000000002'),
('b1c2d3e4-0008-0000-0000-000000000006', 'a1b2c3d4-0008-0000-0000-000000000006', 'c1d2e3f4-0005-0000-0000-000000000005'),
('b1c2d3e4-0009-0000-0000-000000000006', 'a1b2c3d4-0009-0000-0000-000000000006', 'c1d2e3f4-0003-0000-0000-000000000003'),
('b1c2d3e4-0010-0000-0000-000000000006', 'a1b2c3d4-0010-0000-0000-000000000006', 'c1d2e3f4-0004-0000-0000-000000000004');

