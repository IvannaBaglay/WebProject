describe("Test Login", () => {
    beforeEach(() => {
        cy.visit('http://localhost:8081/login')
      })
    
    it("Test Login", () => {
        cy.get('[type="login"]')
        .type("test@ukr.net").should("have.value", "test@ukr.net")
        cy.get('[type="password"]')
        .type("test").should('have.value', "test")
    })

})