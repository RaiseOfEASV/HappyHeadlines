USE Australia;
GO
-- Seed Articles (Australia)
INSERT INTO [Articles] ([ArticleId], [Name], [Content], [Timestamp]) VALUES
('a1b2c3d4-0001-0000-0000-000000000005', '[Australia] The Rise of Renewable Energy', 'Australia is undergoing a dramatic energy transition, with rooftop solar adoption among the highest in the world. Large-scale wind and solar farms in South Australia are leading the way to a carbon-neutral grid.', '2026-01-05 08:00:00'),
('a1b2c3d4-0002-0000-0000-000000000005', '[Australia] Advances in Artificial Intelligence', 'Australian universities and research centres are contributing to global AI advancements in agriculture, mining, and healthcare. Government-backed AI initiatives are helping industries adopt machine learning at scale.', '2026-01-12 09:30:00'),
('a1b2c3d4-0003-0000-0000-000000000005', '[Australia] Space Tourism Takes Off', 'Australia''s vast outback is being eyed as a launch site for commercial space missions. The Australian Space Agency is partnering with international firms to develop launch infrastructure in remote areas.', '2026-01-18 10:15:00'),
('a1b2c3d4-0004-0000-0000-000000000005', '[Australia] The Future of Remote Work', 'Australia''s geography makes remote work a natural fit, and the trend has accelerated since the pandemic. Regional towns are seeing population growth as workers relocate from major cities like Sydney and Melbourne.', '2026-01-25 11:00:00'),
('a1b2c3d4-0005-0000-0000-000000000005', '[Australia] Breakthroughs in Cancer Research', 'Australian researchers at institutions like the Peter MacCallum Cancer Centre are making strides in melanoma treatment. New immunotherapy protocols developed in Australia are gaining international recognition.', '2026-02-02 08:45:00'),
('a1b2c3d4-0006-0000-0000-000000000005', '[Australia] Australian Food Security Challenges', 'Prolonged droughts and bushfires are testing Australia''s agricultural resilience. Farmers are turning to drought-tolerant crops, precision irrigation, and carbon farming to adapt to a harsher climate.', '2026-02-10 13:00:00'),
('a1b2c3d4-0007-0000-0000-000000000005', '[Australia] Quantum Computing Milestones', 'Australia is a global leader in quantum computing research, with Silicon Quantum Computing and Q-CTRL at the forefront. Government investment is accelerating the path to a commercially viable quantum computer.', '2026-02-17 14:30:00'),
('a1b2c3d4-0008-0000-0000-000000000005', '[Australia] Mental Health Awareness in the Digital Age', 'Australia has developed world-class digital mental health services including Beyond Blue and Headspace. Online platforms are reaching people in remote and rural areas who previously lacked access to support.', '2026-02-24 09:00:00'),
('a1b2c3d4-0009-0000-0000-000000000005', '[Australia] Electric Vehicles Dominate Auto Market', 'EV adoption in Australia is accelerating rapidly, driven by new government incentives and a growing charging network. Australian-made battery technology is also attracting investment from global automakers.', '2026-03-03 10:00:00'),
('a1b2c3d4-0010-0000-0000-000000000005', '[Australia] Cybersecurity in a Connected World', 'Australia has experienced several high-profile data breaches, prompting a major overhaul of the national cybersecurity strategy. The government is mandating stronger data protection standards across critical sectors.', '2026-03-09 11:30:00');

-- Seed ArticleAuthors (Australia)
INSERT INTO [ArticleAuthors] ([Id], [ArticleId], [AuthorId]) VALUES
('b1c2d3e4-0001-0000-0000-000000000005', 'a1b2c3d4-0001-0000-0000-000000000005', 'c1d2e3f4-0001-0000-0000-000000000001'),
('b1c2d3e4-0002-0000-0000-000000000005', 'a1b2c3d4-0002-0000-0000-000000000005', 'c1d2e3f4-0002-0000-0000-000000000002'),
('b1c2d3e4-0003-0000-0000-000000000005', 'a1b2c3d4-0003-0000-0000-000000000005', 'c1d2e3f4-0003-0000-0000-000000000003'),
('b1c2d3e4-0004-0000-0000-000000000005', 'a1b2c3d4-0004-0000-0000-000000000005', 'c1d2e3f4-0001-0000-0000-000000000001'),
('b1c2d3e4-0005-0000-0000-000000000005', 'a1b2c3d4-0005-0000-0000-000000000005', 'c1d2e3f4-0004-0000-0000-000000000004'),
('b1c2d3e4-0006-0000-0000-000000000005', 'a1b2c3d4-0006-0000-0000-000000000005', 'c1d2e3f4-0005-0000-0000-000000000005'),
('b1c2d3e4-0007-0000-0000-000000000005', 'a1b2c3d4-0007-0000-0000-000000000005', 'c1d2e3f4-0002-0000-0000-000000000002'),
('b1c2d3e4-0008-0000-0000-000000000005', 'a1b2c3d4-0008-0000-0000-000000000005', 'c1d2e3f4-0005-0000-0000-000000000005'),
('b1c2d3e4-0009-0000-0000-000000000005', 'a1b2c3d4-0009-0000-0000-000000000005', 'c1d2e3f4-0003-0000-0000-000000000003'),
('b1c2d3e4-0010-0000-0000-000000000005', 'a1b2c3d4-0010-0000-0000-000000000005', 'c1d2e3f4-0004-0000-0000-000000000004');

