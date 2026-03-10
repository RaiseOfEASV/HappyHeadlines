USE Global;
GO

-- Seed Articles
INSERT INTO [Articles] ([ArticleId], [Name], [Content], [Timestamp]) VALUES
('a1b2c3d4-0001-0000-0000-000000000001', 'The Rise of Renewable Energy', 'Renewable energy sources such as solar, wind, and hydro are transforming the global power grid. Countries worldwide are investing heavily in clean energy infrastructure to combat climate change and reduce carbon emissions.', '2026-01-05 08:00:00'),
('a1b2c3d4-0002-0000-0000-000000000002', '[Global] Advances in Artificial Intelligence', 'Artificial intelligence continues to reshape industries from healthcare to finance. Large language models and generative AI tools are becoming mainstream, raising important questions about ethics, employment, and regulation.', '2026-01-12 09:30:00'),
('a1b2c3d4-0003-0000-0000-000000000003', '[Global] Space Tourism Takes Off', 'Private companies are making space travel a reality for civilians. With multiple successful crewed missions completed, the commercial space tourism industry is expected to grow exponentially over the next decade.', '2026-01-18 10:15:00'),
('a1b2c3d4-0004-0000-0000-000000000004', '[Global] The Future of Remote Work', 'Remote work has permanently altered the employment landscape. Businesses are adapting their policies, culture, and technology stacks to support distributed teams while maintaining productivity and collaboration.', '2026-01-25 11:00:00'),
('a1b2c3d4-0005-0000-0000-000000000005', '[Global] Breakthroughs in Cancer Research', 'Scientists have announced significant progress in targeted cancer therapies. New immunotherapy treatments show remarkable success rates in clinical trials, offering hope to millions of patients worldwide.', '2026-02-02 08:45:00'),
('a1b2c3d4-0006-0000-0000-000000000006', '[Global] Global Food Security Challenges', 'Climate change and population growth are straining the world''s food supply. Innovative agricultural technologies, including vertical farming and lab-grown proteins, are emerging as potential solutions.', '2026-02-10 13:00:00'),
('a1b2c3d4-0007-0000-0000-000000000007', '[Global] Quantum Computing Milestones', 'Quantum computers have achieved new performance benchmarks, surpassing classical computers on specific problem sets. Tech giants and startups alike are racing to develop practical quantum applications.', '2026-02-17 14:30:00'),
('a1b2c3d4-0008-0000-0000-000000000008', '[Global] Mental Health Awareness in the Digital Age', 'Rising rates of anxiety and depression are prompting a global conversation about mental health. Digital platforms and apps are playing a dual role — both contributing to stress and offering new tools for therapy and support.', '2026-02-24 09:00:00'),
('a1b2c3d4-0009-0000-0000-000000000009', '[Global] Electric Vehicles Dominate Auto Market', 'Electric vehicle sales have surpassed internal combustion engine cars in several major markets. Government incentives, expanding charging infrastructure, and falling battery costs are fuelling rapid adoption.', '2026-03-03 10:00:00'),
('a1b2c3d4-0010-0000-0000-000000000010', '[Global] Cybersecurity in a Connected World', 'As more devices connect to the internet, cybersecurity threats are growing in scale and sophistication. Organisations are urged to adopt zero-trust security models and invest in employee awareness training.', '2026-03-09 11:30:00');

-- Seed ArticleAuthors (Global)
INSERT INTO [ArticleAuthors] ([Id], [ArticleId], [AuthorId]) VALUES
('b1c2d3e4-0001-0000-0000-000000000001', 'a1b2c3d4-0001-0000-0000-000000000001', 'c1d2e3f4-0001-0000-0000-000000000001'),
('b1c2d3e4-0002-0000-0000-000000000002', 'a1b2c3d4-0002-0000-0000-000000000002', 'c1d2e3f4-0002-0000-0000-000000000002'),
('b1c2d3e4-0003-0000-0000-000000000003', 'a1b2c3d4-0003-0000-0000-000000000003', 'c1d2e3f4-0003-0000-0000-000000000003'),
('b1c2d3e4-0004-0000-0000-000000000004', 'a1b2c3d4-0004-0000-0000-000000000004', 'c1d2e3f4-0001-0000-0000-000000000001'),
('b1c2d3e4-0005-0000-0000-000000000005', 'a1b2c3d4-0005-0000-0000-000000000005', 'c1d2e3f4-0004-0000-0000-000000000004'),
('b1c2d3e4-0006-0000-0000-000000000006', 'a1b2c3d4-0006-0000-0000-000000000006', 'c1d2e3f4-0005-0000-0000-000000000005'),
('b1c2d3e4-0007-0000-0000-000000000007', 'a1b2c3d4-0007-0000-0000-000000000007', 'c1d2e3f4-0002-0000-0000-000000000002'),
('b1c2d3e4-0008-0000-0000-000000000008', 'a1b2c3d4-0008-0000-0000-000000000008', 'c1d2e3f4-0005-0000-0000-000000000005'),
('b1c2d3e4-0009-0000-0000-000000000009', 'a1b2c3d4-0009-0000-0000-000000000009', 'c1d2e3f4-0003-0000-0000-000000000003'),
('b1c2d3e4-0010-0000-0000-000000000010', 'a1b2c3d4-0010-0000-0000-000000000010', 'c1d2e3f4-0004-0000-0000-000000000004');

