### СХЕМА СВЯЗЕЙ

Project (1) ←→ (Many) Teams
Project (1) ←→ (Many) Tasks  
Project (1) ←→ (1) GameConcept
Project (1) ←→ (1) TechnicalRequirements
Project (1) ←→ (Many) Assets
Project (1) ←→ (Many) BugReports

Team (1) ←→ (Many) Employees

Employee (1) ←→ (Many) Tasks (Assigned)
Tester (1) ←→ (Many) BugReports

Employee ←─ GameDesigner, Developer, Artist, Tester, Producer (наследование)