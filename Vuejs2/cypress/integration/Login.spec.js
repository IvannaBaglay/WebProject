describe("Test Login", () => {
    beforeEach(() => {
        cy.visit('http://localhost:8081/login')
      })
    
    it("Test Login", () => {
        cy.get('[type="login"]')
        .click()
        .type("test@ukr.net").should("have.value", "test@ukr.net")
        cy.get('[type="password"]')
        .click()
        .type("test").should('have.value', "test")
    })

})