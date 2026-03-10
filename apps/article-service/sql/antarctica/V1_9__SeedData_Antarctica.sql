
USE Antarctica;
GO

-- Seed Articles (Antarctica)
INSERT INTO [Articles] ([ArticleId], [Name], [Content], [Timestamp]) VALUES
('a1b2c3d4-0001-0000-0000-000000000008', '[Antarctica] The Rise of Renewable Energy', 'Antarctic research stations are pioneering 100% renewable energy operations. Wind turbines and solar panels now power bases that previously relied on diesel generators, demonstrating clean energy viability in extreme conditions.', '2026-01-05 08:00:00'),
('a1b2c3d4-0002-0000-0000-000000000008', '[Antarctica] Advances in Artificial Intelligence', 'AI systems are being deployed at Antarctic research stations to autonomously monitor ice sheets, wildlife populations, and atmospheric conditions. Machine learning models are accelerating the analysis of decades of climate data.', '2026-01-12 09:30:00'),
('a1b2c3d4-0003-0000-0000-000000000008', '[Antarctica] Space Tourism Takes Off', 'The skies above Antarctica offer some of the clearest views for astronomical observation and aurora tourism. Research vessels and specialist operators are offering once-in-a-lifetime expeditions to the frozen continent.', '2026-01-18 10:15:00'),
('a1b2c3d4-0004-0000-0000-000000000008', '[Antarctica] The Future of Remote Work', 'Antarctica represents the ultimate test of remote work, with researchers living and working in complete isolation for months. Satellite internet improvements are transforming connectivity at the bottom of the world.', '2026-01-25 11:00:00'),
('a1b2c3d4-0005-0000-0000-000000000008', '[Antarctica] Breakthroughs in Cancer Research', 'Organisms surviving in Antarctica''s extreme cold are yielding novel biological compounds with potential cancer-fighting properties. Researchers are studying antifreeze proteins and extremophiles for pharmaceutical applications.', '2026-02-02 08:45:00'),
('a1b2c3d4-0006-0000-0000-000000000008', '[Antarctica] Antarctic Food Security Insights', 'Antarctic research stations are experimenting with closed-loop hydroponic systems to grow fresh produce year-round. These innovations, born of necessity, offer valuable insights for food production in other resource-scarce environments.', '2026-02-10 13:00:00'),
('a1b2c3d4-0007-0000-0000-000000000008', '[Antarctica] Quantum Computing Milestones', 'Antarctica''s extreme cold makes it a theoretically ideal environment for quantum hardware. Scientists are exploring whether superconducting quantum processors could benefit from the continent''s naturally low temperatures.', '2026-02-17 14:30:00'),
('a1b2c3d4-0008-0000-0000-000000000008', '[Antarctica] Mental Health Awareness in the Digital Age', 'Extended isolation at Antarctic research stations presents unique mental health challenges. Psychologists and space agencies are studying Antarctic crews to develop protocols for long-duration space missions.', '2026-02-24 09:00:00'),
('a1b2c3d4-0009-0000-0000-000000000008', '[Antarctica] Electric Vehicles Dominate Auto Market', 'Electric snowmobiles and tracked vehicles are beginning to replace diesel-powered transport at Antarctic stations. Battery performance in sub-zero temperatures is being refined through real-world testing on the ice.', '2026-03-03 10:00:00'),
('a1b2c3d4-0010-0000-0000-000000000008', '[Antarctica] Cybersecurity in a Connected World', 'As Antarctic research stations become more connected, cybersecurity has become a concern even at the end of the earth. Satellite communication links require robust encryption to protect sensitive scientific and logistical data.', '2026-03-09 11:30:00');

-- Seed ArticleAuthors (Antarctica)
INSERT INTO [ArticleAuthors] ([Id], [ArticleId], [AuthorId]) VALUES
('b1c2d3e4-0001-0000-0000-000000000008', 'a1b2c3d4-0001-0000-0000-000000000008', 'c1d2e3f4-0001-0000-0000-000000000001'),
('b1c2d3e4-0002-0000-0000-000000000008', 'a1b2c3d4-0002-0000-0000-000000000008', 'c1d2e3f4-0002-0000-0000-000000000002'),
('b1c2d3e4-0003-0000-0000-000000000008', 'a1b2c3d4-0003-0000-0000-000000000008', 'c1d2e3f4-0003-0000-0000-000000000003'),
('b1c2d3e4-0004-0000-0000-000000000008', 'a1b2c3d4-0004-0000-0000-000000000008', 'c1d2e3f4-0001-0000-0000-000000000001'),
('b1c2d3e4-0005-0000-0000-000000000008', 'a1b2c3d4-0005-0000-0000-000000000008', 'c1d2e3f4-0004-0000-0000-000000000004'),
('b1c2d3e4-0006-0000-0000-000000000008', 'a1b2c3d4-0006-0000-0000-000000000008', 'c1d2e3f4-0005-0000-0000-000000000005'),
('b1c2d3e4-0007-0000-0000-000000000008', 'a1b2c3d4-0007-0000-0000-000000000008', 'c1d2e3f4-0002-0000-0000-000000000002'),
('b1c2d3e4-0008-0000-0000-000000000008', 'a1b2c3d4-0008-0000-0000-000000000008', 'c1d2e3f4-0005-0000-0000-000000000005'),
('b1c2d3e4-0009-0000-0000-000000000008', 'a1b2c3d4-0009-0000-0000-000000000008', 'c1d2e3f4-0003-0000-0000-000000000003'),
('b1c2d3e4-0010-0000-0000-000000000008', 'a1b2c3d4-0010-0000-0000-000000000008', 'c1d2e3f4-0004-0000-0000-000000000004');

