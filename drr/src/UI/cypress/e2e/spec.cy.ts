describe('EOI application happy path', () => {
  it('Fill whole application and hold on step 8', () => {
    cy.visit('/');
    // click start button
    cy.get('button').click();

    // step 1
    // check radio button
    cy.get('#applicantType [type="radio"]').first().check();
    // add applicant name by id attr
    cy.get('#applicantName').type('New Applicant Organization', {
      force: true,
    });

    // fill submitter details
    cy.get('#submitter_firstName').type('John');
    cy.get('#submitter_lastName').type('Doe');
    cy.get('#submitter_title').type('Sr Manager');
    cy.get('#submitter_department').type('Innovation and Renovation');
    cy.get('#submitter_phone').type('123-456-7890');
    cy.get('#submitter_email').type('john.doe@example.com');

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

    // click next button
    cy.get('#next1').click();

    // step 2
    cy.get('#projectTitle').type('New Project Title');
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
    cy.get('mat-option').contains('Fed').click();
    cy.get('#otherFunding_type_1').type('{esc}');
    cy.get('#otherFunding_amount_1').type('50000');

    cy.get('#unfundedAmount').type('20000', { force: true });

    cy.get('#reasonsToSecureFunding').type(
      'I will borrow some money from my aunt and uncle'
    );

    // click next button
    cy.get('#next3').click();

    // step 4
    cy.get('#ownershipDeclaration [type="radio"]').first().check();
    cy.get('#locationDescription').type('This is a very beautiful location', {
      force: true,
    });
    cy.get('#locationArea').type('1000 sq m', { force: true });
    cy.get('#locationOwnership').type('Owned by me and my family', {
      force: true,
    });

    // click next button
    cy.get('#next4').click();

    // step 5
    cy.get('#backgroundDescription').type(
      'This is a very beautiful background'
    );
    cy.get('#rationaleForFunding').type('I need money to build a house');
    cy.get('#rationaleForSolution').type('I need a house to live in');
    cy.get('#proposedSolution').type('I will build a house');

    // click next button
    cy.get('#next5').click();

    // step 6
    cy.get('#engagementProposal').type('I will engage with the community');

    // click next button
    cy.get('#next6').click();

    // step 7
    cy.get('#climateAdaptation').type('I will adapt to the climate');
    cy.get('#otherInformation').type('Some other information');

    // click next button
    cy.get('#next7').click();
  });
});
