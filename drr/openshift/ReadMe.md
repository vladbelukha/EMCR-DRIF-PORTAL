EMCR - DRR
======================

#Parameter Files

Parameter files will have the following format:

`ENV=test`
`TAG=test`

To use a parameter file when importing a template:

`oc process -f drr.template.yml --param-file="../../../emcr-drr-params/test.params" | oc create -f -`

(Substitute "emcr-drr-params" for the folder where the parameter file is stored)
