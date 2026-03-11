USE Europe;
GO
-- Seed Articles (Europe)
INSERT INTO [Articles] ([ArticleId], [Name], [Content], [Timestamp]) VALUES
('a1b2c3d4-0001-0000-0000-000000000002', '[Europe] The Rise of Renewable Energy', 'Renewable energy sources such as solar, wind, and hydro are transforming the European power grid. Countries across the continent are investing heavily in clean energy infrastructure to meet EU climate targets.', '2026-01-05 08:00:00'),
('a1b2c3d4-0002-0000-0000-000000000002', '[Europe] Advances in Artificial Intelligence', 'Europe''s AI Act is setting the global standard for AI regulation. Tech hubs in Berlin, Paris, and Amsterdam are driving innovation while ensuring ethical and transparent use of AI systems.', '2026-01-12 09:30:00'),
('a1b2c3d4-0003-0000-0000-000000000002', '[Europe] Space Tourism Takes Off', 'European aerospace agencies and private companies are competing for a slice of the growing space tourism market. Launch facilities in the UK and Sweden are being developed to support civilian missions.', '2026-01-18 10:15:00'),
('a1b2c3d4-0004-0000-0000-000000000002', '[Europe] The Future of Remote Work', 'European labour laws are evolving to protect remote workers'' rights. Countries like Portugal and Germany have introduced right-to-disconnect legislation, shaping the future of flexible employment.', '2026-01-25 11:00:00'),
('a1b2c3d4-0005-0000-0000-000000000002', '[Europe] Breakthroughs in Cancer Research', 'Research centres across Europe are leading groundbreaking cancer studies. The European Cancer Mission aims to save 3 million lives by 2030 through early detection and targeted treatment programmes.', '2026-02-02 08:45:00'),
('a1b2c3d4-0006-0000-0000-000000000002', '[Europe] European Food Security Challenges', 'Extreme weather events across Southern Europe are disrupting agricultural output. The EU is funding sustainable farming initiatives and seed vaults to safeguard food security for future generations.', '2026-02-10 13:00:00'),
('a1b2c3d4-0007-0000-0000-000000000002', '[Europe] Quantum Computing Milestones', 'The European Quantum Flagship programme has reached key milestones in building a continent-wide quantum network. Universities in Delft and Vienna are leading efforts in quantum communication and computing.', '2026-02-17 14:30:00'),
('a1b2c3d4-0008-0000-0000-000000000002', '[Europe] Mental Health Awareness in the Digital Age', 'European health ministries are increasing funding for mental health services. Nordic countries continue to lead in workplace wellbeing initiatives, setting a model for the rest of the continent.', '2026-02-24 09:00:00'),
('a1b2c3d4-0009-0000-0000-000000000002', '[Europe] Electric Vehicles Dominate Auto Market', 'Europe''s ban on new petrol and diesel car sales by 2035 is accelerating EV adoption. German and French manufacturers are unveiling next-generation electric models to lead the transition.', '2026-03-03 10:00:00'),
('a1b2c3d4-0010-0000-0000-000000000002', '[Europe] Cybersecurity in a Connected World', 'The EU''s NIS2 Directive is strengthening cybersecurity standards across member states. Critical infrastructure operators must now comply with stricter reporting and resilience requirements.', '2026-03-09 11:30:00');


INSERT INTO [ArticleAuthors] ([Id], [ArticleId], [AuthorId]) VALUES
-- Article 1
    (NEWID(),'a1b2c3d4-0001-0000-0000-000000000002','c1d2e3f4-0001-0000-0000-000000000001'),
    (NEWID(),'a1b2c3d4-0001-0000-0000-000000000002','c1d2e3f4-0002-0000-0000-000000000002'),
    (NEWID(),'a1b2c3d4-0001-0000-0000-000000000002','c1d2e3f4-0003-0000-0000-000000000003'),

-- Article 2
    (NEWID(),'a1b2c3d4-0002-0000-0000-000000000002','c1d2e3f4-0002-0000-0000-000000000002'),
    (NEWID(),'a1b2c3d4-0002-0000-0000-000000000002','c1d2e3f4-0003-0000-0000-000000000003'),
    (NEWID(),'a1b2c3d4-0002-0000-0000-000000000002','c1d2e3f4-0004-0000-0000-000000000004'),

-- Article 3
    (NEWID(),'a1b2c3d4-0003-0000-0000-000000000002','c1d2e3f4-0003-0000-0000-000000000003'),
    (NEWID(),'a1b2c3d4-0003-0000-0000-000000000002','c1d2e3f4-0004-0000-0000-000000000004'),
    (NEWID(),'a1b2c3d4-0003-0000-0000-000000000002','c1d2e3f4-0005-0000-0000-000000000005'),

-- Article 4
    (NEWID(),'a1b2c3d4-0004-0000-0000-000000000002','c1d2e3f4-0001-0000-0000-000000000001'),
    (NEWID(),'a1b2c3d4-0004-0000-0000-000000000002','c1d2e3f4-0004-0000-0000-000000000004'),
    (NEWID(),'a1b2c3d4-0004-0000-0000-000000000002','c1d2e3f4-0005-0000-0000-000000000005'),

-- Article 5
    (NEWID(),'a1b2c3d4-0005-0000-0000-000000000002','c1d2e3f4-0004-0000-0000-000000000004'),
    (NEWID(),'a1b2c3d4-0005-0000-0000-000000000002','c1d2e3f4-0005-0000-0000-000000000005'),
    (NEWID(),'a1b2c3d4-0005-0000-0000-000000000002','c1d2e3f4-0001-0000-0000-000000000001'),

-- Article 6
    (NEWID(),'a1b2c3d4-0006-0000-0000-000000000002','c1d2e3f4-0005-0000-0000-000000000005'),
    (NEWID(),'a1b2c3d4-0006-0000-0000-000000000002','c1d2e3f4-0001-0000-0000-000000000001'),
    (NEWID(),'a1b2c3d4-0006-0000-0000-000000000002','c1d2e3f4-0002-0000-0000-000000000002'),

-- Article 7
    (NEWID(),'a1b2c3d4-0007-0000-0000-000000000002','c1d2e3f4-0002-0000-0000-000000000002'),
    (NEWID(),'a1b2c3d4-0007-0000-0000-000000000002','c1d2e3f4-0003-0000-0000-000000000003'),
    (NEWID(),'a1b2c3d4-0007-0000-0000-000000000002','c1d2e3f4-0004-0000-0000-000000000004'),

-- Article 8
    (NEWID(),'a1b2c3d4-0008-0000-0000-000000000002','c1d2e3f4-0005-0000-0000-000000000005'),
    (NEWID(),'a1b2c3d4-0008-0000-0000-000000000002','c1d2e3f4-0001-0000-0000-000000000001'),
    (NEWID(),'a1b2c3d4-0008-0000-0000-000000000002','c1d2e3f4-0002-0000-0000-000000000002'),

-- Article 9
    (NEWID(),'a1b2c3d4-0009-0000-0000-000000000002','c1d2e3f4-0003-0000-0000-000000000003'),
    (NEWID(),'a1b2c3d4-0009-0000-0000-000000000002','c1d2e3f4-0002-0000-0000-000000000002'),
    (NEWID(),'a1b2c3d4-0009-0000-0000-000000000002','c1d2e3f4-0004-0000-0000-000000000004'),

-- Article 10
    (NEWID(),'a1b2c3d4-0010-0000-0000-000000000002','c1d2e3f4-0004-0000-0000-000000000004'),
    (NEWID(),'a1b2c3d4-0010-0000-0000-000000000002','c1d2e3f4-0001-0000-0000-000000000001'),
    (NEWID(),'a1b2c3d4-0010-0000-0000-000000000002','c1d2e3f4-0005-0000-0000-000000000005');


