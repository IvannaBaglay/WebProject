describe('Test Links', ()=>
{
    beforeEach(()=>{
        cy.visit('http://localhost:8081/')
    })
    it('check links', ()=> {
        cy.contains('h1', 'Home');
    })
})
