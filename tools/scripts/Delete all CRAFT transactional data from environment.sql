--********************************************************************
--** Delete all CRAFT transactional data from the Dynamics environment
--********************************************************************

RAISERROR('Delete all CRAFT transactional data from the Dynamics environment ...', 0, 1) WITH NOWAIT;

-- Application Child entities
--
RAISERROR('Delete account ...', 0, 1) WITH NOWAIT;
delete from account --proponent
where name is not null

--***** cascade delete when all accounts deleted? Yes**********
RAISERROR('Delete connection ...', 0, 1) WITH NOWAIT;
delete from connection  --partnering proponents
where 1=1

RAISERROR('Delete contact ...', 0, 1) WITH NOWAIT;
delete from contact --proponent contacts
where drr_contacttypename is null --preserve SME contact types

RAISERROR('Delete drr_applicationstatushistory ...', 0, 1) WITH NOWAIT;
delete from drr_applicationstatushistory
where drr_application is not null

RAISERROR('Delete drr_climateassessmenttoolitem ...', 0, 1) WITH NOWAIT;
delete from drr_climateassessmenttoolitem
where drr_application is not null

RAISERROR('Delete drr_cobenefititem ...', 0, 1) WITH NOWAIT;
delete from drr_cobenefititem
where drr_application is not null

RAISERROR('Delete drr_costconsiderationitem ...', 0, 1) WITH NOWAIT;
delete from drr_costconsiderationitem
where drr_application is not null

RAISERROR('Delete drr_costreductionitem ...', 0, 1) WITH NOWAIT;
delete from drr_costreductionitem
where drr_application is not null

RAISERROR('Delete drr_criticalinfrastructureimpacted ...', 0, 1) WITH NOWAIT;
delete from drr_criticalinfrastructureimpacted
where drr_application is not null

RAISERROR('Delete drr_driffundingrequest ...', 0, 1) WITH NOWAIT;
delete from drr_driffundingrequest
where drr_application is not null

RAISERROR('Delete drr_eligibilityscreening ...', 0, 1) WITH NOWAIT;
delete from drr_eligibilityscreening
where drr_name is not null

RAISERROR('Delete drr_eoiscoring ...', 0, 1) WITH NOWAIT;
delete from drr_eoiscoring
where drr_application is not null

RAISERROR('Delete drr_foundationalorpreviousworkitem...', 0, 1) WITH NOWAIT;
delete from drr_foundationalorpreviousworkitem
where drr_application is not null

RAISERROR('Delete drr_fundingsource...', 0, 1) WITH NOWAIT;
delete from drr_fundingsource
where drr_application is not null

RAISERROR('Delete drr_impactedoraffectedpartyitem...', 0, 1) WITH NOWAIT;
delete from drr_impactedoraffectedpartyitem
where drr_application is not null

RAISERROR('Delete drr_permitslicensesandauthorizations ...', 0, 1) WITH NOWAIT;
delete from drr_permitslicensesandauthorizations
where drr_application is not null

RAISERROR('Delete drr_projectcapacitychallengeitem ...', 0, 1) WITH NOWAIT;
delete from drr_projectcapacitychallengeitem
where drr_application is not null

RAISERROR('Delete drr_projectcomplexityriskitem ...', 0, 1) WITH NOWAIT;
delete from drr_projectcomplexityriskitem
where drr_application is not null

RAISERROR('Delete drr_projectneedidentificationitem ...', 0, 1) WITH NOWAIT;
delete from drr_projectneedidentificationitem
where drr_name is not null

RAISERROR('Delete drr_projectreadinessriskitem ...', 0, 1) WITH NOWAIT;
delete from drr_projectreadinessriskitem
where drr_name is not null

RAISERROR('Delete drr_projectsensitivityriskitem ...', 0, 1) WITH NOWAIT;
delete from drr_projectsensitivityriskitem
where drr_name is not null

RAISERROR('Delete drr_proposedactivity ...', 0, 1) WITH NOWAIT;
delete from drr_proposedactivity
where drr_name is not null

RAISERROR('Delete drr_provincialstandarditem ...', 0, 1) WITH NOWAIT;
delete from drr_provincialstandarditem
where drr_name is not null

RAISERROR('Delete drr_qualifiedprofessionalitem ...', 0, 1) WITH NOWAIT;
delete from drr_qualifiedprofessionalitem
where drr_name is not null

RAISERROR('Delete drr_resiliencyitem ...', 0, 1) WITH NOWAIT;
delete from drr_resiliencyitem
where drr_name is not null

RAISERROR('Delete drr_smereview ...', 0, 1) WITH NOWAIT;
delete from drr_smereview
where drr_name is not null

RAISERROR('Delete drr_smereviewitem ...', 0, 1) WITH NOWAIT;
delete from drr_smereviewitem
where drr_name is not null

RAISERROR('Delete bcgov_documenturl ...', 0, 1) WITH NOWAIT;
delete from bcgov_documenturl --attachments
where 1=1

--RAISERROR('Delete drr_project ...', 0, 1) WITH NOWAIT;
--delete from drr_project
--where 1=1

-- Application Related entities
--
RAISERROR('Delete incident ...', 0, 1) WITH NOWAIT;
delete from incident --cases
where 1=1

-- Application entity
--
RAISERROR('Delete drr_application ...', 0, 1) WITH NOWAIT;
delete from drr_application
where drr_name is not null

RAISERROR('Completed all Delete statements', 0, 1) WITH NOWAIT;
