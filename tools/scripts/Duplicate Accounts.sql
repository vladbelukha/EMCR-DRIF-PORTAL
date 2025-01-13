
--==========================================================================
-- sql statements to identify duplicate accounts and associated applications
--==========================================================================

--
-- duplicate account names and number of duplicates
--
select count(1) account_records, name proponent_name from account group by name having account_records > 1 order by name

--
-- duplicate account details
--
select name, accountid, drr_bceidguid, createdon, createdbyname, statecodename
from account where name in
( select sub.proponent_name from
    (select count(1) account_records, name proponent_name 
     from account group by name having account_records > 1) sub)

--
-- applications associated with duplicate accounts
--
select
  drr_applicationid
  ,drr_applicationtypename
  ,drr_autonumber
  ,drr_eoiapplicationname
  ,drr_fullproposalapplicationname
  ,drr_primary_proponent_namename
  ,drr_primary_proponent_name
  ,statuscodename
  ,createdon
  ,createdbyname
from drr_application where drr_primary_proponent_namename in 
  (select sub.proponent_name from
    (select count(1) account_records, name proponent_name 
     from account group by name having account_records > 1) sub)