USE Asia;
GO
-- Seed Articles (Asia)
INSERT INTO [Articles] ([ArticleId], [Name], [Content], [Timestamp]) VALUES
('a1b2c3d4-0001-0000-0000-000000000004', '[Asia] The Rise of Renewable Energy', 'Asia is the world''s largest and fastest-growing renewable energy market. China leads global solar panel production, while India is rapidly expanding its wind and solar capacity to power its growing economy.', '2026-01-05 08:00:00'),
('a1b2c3d4-0002-0000-0000-000000000004', '[Asia] Advances in Artificial Intelligence', 'Asian tech giants in China, South Korea, and Japan are competing fiercely in the global AI race. Generative AI tools developed in the region are being adopted across manufacturing, logistics, and customer service.', '2026-01-12 09:30:00'),
('a1b2c3d4-0003-0000-0000-000000000004', '[Asia] Space Tourism Takes Off', 'China and Japan are advancing ambitious space programmes, including lunar exploration and planned space stations. Private companies in Singapore and India are also entering the commercial space tourism market.', '2026-01-18 10:15:00'),
('a1b2c3d4-0004-0000-0000-000000000004', '[Asia] The Future of Remote Work', 'Asian corporations are navigating the tension between traditional office culture and the global shift to remote work. South Korea and Singapore are experimenting with hybrid models to attract international talent.', '2026-01-25 11:00:00'),
('a1b2c3d4-0005-0000-0000-000000000004', '[Asia] Breakthroughs in Cancer Research', 'Research institutions in Japan and South Korea are pioneering early cancer detection technologies using AI-driven imaging. Clinical trials in the region show promising results for liver and lung cancer therapies.', '2026-02-02 08:45:00'),
('a1b2c3d4-0006-0000-0000-000000000004', '[Asia] Asian Food Security Challenges', 'Rapid urbanisation and shifting diets are straining Asia''s food systems. Countries across Southeast Asia are investing in aquaculture, precision farming, and alternative proteins to meet demand.', '2026-02-10 13:00:00'),
('a1b2c3d4-0007-0000-0000-000000000004', '[Asia] Quantum Computing Milestones', 'China has demonstrated significant advances in quantum communication, launching the world''s first quantum satellite network. Japan and South Korea are accelerating investment in quantum hardware and software.', '2026-02-17 14:30:00'),
('a1b2c3d4-0008-0000-0000-000000000004', '[Asia] Mental Health Awareness in the Digital Age', 'Mental health awareness is growing across Asia, challenging longstanding cultural stigmas. Digital therapy platforms and workplace wellness programmes are gaining traction in Japan, South Korea, and India.', '2026-02-24 09:00:00'),
('a1b2c3d4-0009-0000-0000-000000000004', '[Asia] Electric Vehicles Dominate Auto Market', 'China is the undisputed global leader in EV sales and manufacturing. BYD and other Chinese brands are expanding into Southeast Asian markets, while India is rapidly scaling up its own EV ecosystem.', '2026-03-03 10:00:00'),
('a1b2c3d4-0010-0000-0000-000000000004', '[Asia] Cybersecurity in a Connected World', 'Asia''s hyper-connected societies face escalating cybersecurity threats from state and non-state actors. Governments in Singapore, Japan, and South Korea are tightening regulations and building cyber defence capabilities.', '2026-03-09 11:30:00');

-- Seed ArticleAuthors (Asia)
INSERT INTO [ArticleAuthors] ([Id], [ArticleId], [AuthorId]) VALUES
('b1c2d3e4-0001-0000-0000-000000000004', 'a1b2c3d4-0001-0000-0000-000000000004', 'c1d2e3f4-0001-0000-0000-000000000001'),
('b1c2d3e4-0002-0000-0000-000000000004', 'a1b2c3d4-0002-0000-0000-000000000004', 'c1d2e3f4-0002-0000-0000-000000000002'),
('b1c2d3e4-0003-0000-0000-000000000004', 'a1b2c3d4-0003-0000-0000-000000000004', 'c1d2e3f4-0003-0000-0000-000000000003'),
('b1c2d3e4-0004-0000-0000-000000000004', 'a1b2c3d4-0004-0000-0000-000000000004', 'c1d2e3f4-0001-0000-0000-000000000001'),
('b1c2d3e4-0005-0000-0000-000000000004', 'a1b2c3d4-0005-0000-0000-000000000004', 'c1d2e3f4-0004-0000-0000-000000000004'),
('b1c2d3e4-0006-0000-0000-000000000004', 'a1b2c3d4-0006-0000-0000-000000000004', 'c1d2e3f4-0005-0000-0000-000000000005'),
('b1c2d3e4-0007-0000-0000-000000000004', 'a1b2c3d4-0007-0000-0000-000000000004', 'c1d2e3f4-0002-0000-0000-000000000002'),
('b1c2d3e4-0008-0000-0000-000000000004', 'a1b2c3d4-0008-0000-0000-000000000004', 'c1d2e3f4-0005-0000-0000-000000000005'),
('b1c2d3e4-0009-0000-0000-000000000004', 'a1b2c3d4-0009-0000-0000-000000000004', 'c1d2e3f4-0003-0000-0000-000000000003'),
('b1c2d3e4-0010-0000-0000-000000000004', 'a1b2c3d4-0010-0000-0000-000000000004', 'c1d2e3f4-0004-0000-0000-000000000004');

