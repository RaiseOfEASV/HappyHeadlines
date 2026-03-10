USE NorthAmerica;
GO
-- Seed Articles (North America)
INSERT INTO [Articles] ([ArticleId], [Name], [Content], [Timestamp]) VALUES
('a1b2c3d4-0001-0000-0000-000000000007', '[North America] The Rise of Renewable Energy', 'The United States and Canada are investing record sums in wind, solar, and hydrogen energy. The Inflation Reduction Act has supercharged clean energy deployment, making North America a global leader in the transition.', '2026-01-05 08:00:00'),
('a1b2c3d4-0002-0000-0000-000000000007', '[North America] Advances in Artificial Intelligence', 'Silicon Valley continues to dominate the global AI landscape, with companies like OpenAI, Google, and Microsoft pushing the boundaries of generative AI. US and Canadian universities are producing world-class AI research talent.', '2026-01-12 09:30:00'),
('a1b2c3d4-0003-0000-0000-000000000007', '[North America] Space Tourism Takes Off', 'SpaceX, Blue Origin, and Virgin Galactic are making space tourism a competitive industry. Launches from Florida and Texas are becoming routine, with ticket prices gradually falling as reusable rocket technology matures.', '2026-01-18 10:15:00'),
('a1b2c3d4-0004-0000-0000-000000000007', '[North America] The Future of Remote Work', 'The remote work revolution that began in North America is reshaping cities and suburbs alike. Major tech companies are renegotiating office policies, while smaller cities benefit from an influx of remote workers seeking affordability.', '2026-01-25 11:00:00'),
('a1b2c3d4-0005-0000-0000-000000000007', '[North America] Breakthroughs in Cancer Research', 'The US National Cancer Institute is funding a new wave of mRNA-based cancer vaccines. Early trials show strong immune responses, raising hopes that personalised cancer vaccines could become mainstream within the decade.', '2026-02-02 08:45:00'),
('a1b2c3d4-0006-0000-0000-000000000007', '[North America] North American Food Security Challenges', 'Supply chain disruptions and climate-driven crop failures are prompting North America to reassess food production strategies. Indoor vertical farming and lab-grown meat startups are attracting significant venture capital investment.', '2026-02-10 13:00:00'),
('a1b2c3d4-0007-0000-0000-000000000007', '[North America] Quantum Computing Milestones', 'IBM, Google, and Microsoft are racing to achieve fault-tolerant quantum computing. The US government has designated quantum technology as a national security priority, injecting billions into research and development programmes.', '2026-02-17 14:30:00'),
('a1b2c3d4-0008-0000-0000-000000000007', '[North America] Mental Health Awareness in the Digital Age', 'Mental health is now firmly on the corporate agenda in North America. Employers are offering expanded mental health benefits, and digital therapy platforms are seeing record usage following post-pandemic awareness campaigns.', '2026-02-24 09:00:00'),
('a1b2c3d4-0009-0000-0000-000000000007', '[North America] Electric Vehicles Dominate Auto Market', 'Tesla, Ford, and GM are battling for EV market share in North America. Federal tax credits and an expanding coast-to-coast charging network are accelerating consumer adoption across the United States and Canada.', '2026-03-03 10:00:00'),
('a1b2c3d4-0010-0000-0000-000000000007', '[North America] Cybersecurity in a Connected World', 'High-profile ransomware attacks on US infrastructure have prompted sweeping federal cybersecurity reforms. CISA is coordinating a national response strategy, mandating security standards for critical sectors.', '2026-03-09 11:30:00');

-- Seed ArticleAuthors (North America)
INSERT INTO [ArticleAuthors] ([Id], [ArticleId], [AuthorId]) VALUES
('b1c2d3e4-0001-0000-0000-000000000007', 'a1b2c3d4-0001-0000-0000-000000000007', 'c1d2e3f4-0001-0000-0000-000000000001'),
('b1c2d3e4-0002-0000-0000-000000000007', 'a1b2c3d4-0002-0000-0000-000000000007', 'c1d2e3f4-0002-0000-0000-000000000002'),
('b1c2d3e4-0003-0000-0000-000000000007', 'a1b2c3d4-0003-0000-0000-000000000007', 'c1d2e3f4-0003-0000-0000-000000000003'),
('b1c2d3e4-0004-0000-0000-000000000007', 'a1b2c3d4-0004-0000-0000-000000000007', 'c1d2e3f4-0001-0000-0000-000000000001'),
('b1c2d3e4-0005-0000-0000-000000000007', 'a1b2c3d4-0005-0000-0000-000000000007', 'c1d2e3f4-0004-0000-0000-000000000004'),
('b1c2d3e4-0006-0000-0000-000000000007', 'a1b2c3d4-0006-0000-0000-000000000007', 'c1d2e3f4-0005-0000-0000-000000000005'),
('b1c2d3e4-0007-0000-0000-000000000007', 'a1b2c3d4-0007-0000-0000-000000000007', 'c1d2e3f4-0002-0000-0000-000000000002'),
('b1c2d3e4-0008-0000-0000-000000000007', 'a1b2c3d4-0008-0000-0000-000000000007', 'c1d2e3f4-0005-0000-0000-000000000005'),
('b1c2d3e4-0009-0000-0000-000000000007', 'a1b2c3d4-0009-0000-0000-000000000007', 'c1d2e3f4-0003-0000-0000-000000000003'),
('b1c2d3e4-0010-0000-0000-000000000007', 'a1b2c3d4-0010-0000-0000-000000000007', 'c1d2e3f4-0004-0000-0000-000000000004');

