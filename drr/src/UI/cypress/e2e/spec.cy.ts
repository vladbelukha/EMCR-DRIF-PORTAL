describe('EOI application happy path', () => {
  it('Fill whole application and hold on step 8', () => {
    cy.visit('/');
    // click start button
    cy.get('button').click();

    // step 1
    // check radio button
    cy.get('#proponentType [type="radio"]').first().check();
    // add applicant name by id attr
    cy.get('#proponentName').type('New Applicant Organization', {
      force: true,
    });

    // fill submitter details
    cy.get('#submitter_firstName').type('John');
    cy.get('#submitter_lastName').type('Doe');
    cy.get('#submitter_title').type('Sr Manager');
    cy.get('#submitter_department').type('Innovation and Renovation');
    cy.get('#submitter_phone').type('123-456-7890');
    cy.get('#submitter_email').type('john.doe@example.com');

    cy.get('#sameAsSubmitter [type="checkbox"]').first().check();

    // add project contacts
    cy.get('#projectContact_firstName_0').type('Jane');
    cy.get('#projectContact_lastName_0').type('Doe');
    cy.get('#projectContact_title_0').type('Project Manager');
    cy.get('#projectContact_department_0').type(
      'Innovation and Renovation, Div 1'
    );
    cy.get('#projectContact_phone_0').type('234-456-7890');
    cy.get('#projectContact_email_0').type('jane.doe@example.com', {});

    // add another project contact
    cy.get('#addProjectContact').click();
    cy.get('#projectContact_firstName_1').type('Mike');
    cy.get('#projectContact_lastName_1').type('Spens');
    cy.get('#projectContact_title_1').type('Jn Project Manager', {});
    cy.get('#projectContact_department_1').type(
      'Innovation and Renovation, Div 1'
    );
    cy.get('#projectContact_phone_1').type('234-456-7890');
    cy.get('#projectContact_email_1').type('mike.spens@example.com', {});

    // add partnering proponents
    cy.get('#proponent_0').type('Proponent 1', { force: true });
    cy.get('#addProponent').click();
    cy.get('#proponent_1').type('Proponent 2', { force: true });

    // click next button
    cy.get('#next1').click();

    // step 2
    cy.get('#fundingStream [type="radio"]').first().check();
    cy.get('#projectTitle').type('New Project Title');
    cy.get('#scopeStatement').type('This is a scope statement');
    cy.get('#projectType [type="radio"]').first().check();

    // select few hazards
    // Open the dropdown
    cy.get('#relatedHazards').click();
    // Select the options
    cy.get('mat-option').contains('Flood').click();
    cy.get('mat-option').contains('Storm').click();
    cy.get('mat-option').contains('Other').click();

    cy.get('#relatedHazards').type('{esc}');

    cy.get('#otherHazardsDescription').type('Something very horrible', {});

    cy.get('#startDate').type('2022-01-01', { force: true });
    cy.get('#endDate').type('2022-12-31', { force: true });

    // click next button
    cy.get('#next2').click();

    // step 3
    cy.get('#estimatedTotal').type('250000');
    cy.get('#fundingRequest').type('100000');
    cy.get('#addOtherFundingButton').click();
    cy.get('#otherFunding_name_0').type('Funding Organization', {
      force: true,
    });
    cy.get('#otherFunding_type_0').click();
    cy.get('mat-option').contains('Fed').click();
    cy.get('#otherFunding_type_0').type('{esc}');
    cy.get('#otherFunding_amount_0').type('50000');

    cy.get('#addOtherFundingButton').click();

    cy.get('#otherFunding_name_1').type('Funding Organization', {
      force: true,
    });
    cy.get('#otherFunding_type_1').click();
    cy.get('mat-option').contains('Other').click();
    cy.get('#otherFunding_type_1').type('{esc}');
    cy.get('#otherFunding_amount_1').type('50000');
    cy.get('#otherFunding_description_1').type('Some description', {
      force: true,
    });

    cy.get('#intendToSecureFunding').type(
      'I will borrow some money from my aunt and uncle'
    );

    // click next button
    cy.get('#next3').click();

    // step 4
    cy.get('#ownershipDeclaration [type="radio"]').first().check();
    cy.get('#ownershipDescription').type('I own the land', {
      force: true,
    });
    cy.get('#locationDescription').type('This is a very beautiful location', {
      force: true,
    });

    // click next button
    cy.get('#next4').click();

    // step 5
    cy.get('#rationaleForFunding').type('I need money to build a house');
    cy.get('#estimatedPeopleImpacted').type('1000');
    cy.get('#communityImpact').type('This will impact the community');
    cy.get('#infrastructureImpacted').type(
      'Some infrastructure will be impacted'
    );
    cy.get('#disasterRiskUnderstanding').type('I understand the risk');
    cy.get('#additionalBackgroundInformation').type(
      'Some additional information'
    );
    cy.get('#addressRisksAndHazards').type('I will address the risks');
    cy.get('#drifProgramGoalAlignment').type('I will align with the goals');
    cy.get('#additionalSolutionInformation').type(
      'Some additional solution information'
    );
    cy.get('#rationaleForSolution').type('I need a house to live in');

    // click next button
    cy.get('#next5').click();

    // step 6
    cy.get('#firstNationsEngagement').type(
      'I will engage with the first nations'
    );
    cy.get('#neighbourEngagement').type('I will engage with the neighbours');
    cy.get('#additionalEngagementInformation').type(
      'Some additional engagement information'
    );

    // click next button
    cy.get('#next6').click();

    // step 7
    cy.get('#climateAdaptation').type('I will adapt to the climate');
    cy.get('#otherInformation').type('Some other information');

    // click next button
    cy.get('#next7').click();

    // step 8
    cy.get('#identityConfirmation [type="checkbox"]').first().check();
    cy.get('#foippaConfirmation [type="checkbox"]').first().check();
    cy.get('#cfoConfirmation [type="checkbox"]').first().check();
  });
});
