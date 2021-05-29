// untitled.spec.js created with Cypress
//
// Start writing your Cypress tests below!
// If you're unfamiliar with how Cypress works,
// check out the link below and learn how to write your first test:
// https://on.cypress.io/writing-first-test

describe('Test Links', ()=>
{
    beforeEach(()=>{
        cy.visit('http://localhost:8081/')
    })
    it('check links', ()=> {
        cy.contains('h1', 'Logged in');
    })
})
