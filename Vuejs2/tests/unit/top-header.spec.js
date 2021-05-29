import TopHeader from '@/components/Top-Header.vue'
import Secret from "@/views/Secret.vue";

import {shallowMount, mount} from '@vue/test-utils'
import flushPromises from 'flush-promises'


const $router = {
    replace: jest.fn()


}

const $axios = {
    get: () => {
      return Promise.resolve({ data: [{ char_id: 1, name: "123" }] });
    }
  };

jest.mock("firebase/app", ()=> ({
    auth(){
        return {
            onAuthStateChanged(fnc){
                return fnc(true);
            },
            signOut: ()=> Promise.resolve(),
            currentUser: {
                getIdToken: () => "blah"
              }
        }
    }
}))

describe ("topHeader.vue", ()=> {
    let wrapper ;
    beforeEach(()=> {
        wrapper = shallowMount(TopHeader, {
            mocks:{
                $router 

            }
            //methods: {setupFirebase: ()=> {}}
        })

    })

    it("renders", ()=> {
        expect(wrapper.exists()).toBe(true);

    })
    it("does h1 exist", ()=> {
        expect(wrapper.find("h1").text()).toBe("Logged in");
    })
    it("user is logged in after setting firebase mock", ()=>{
        expect(wrapper.vm.$data.loggedIn).toBe(true);
    })
    it("on signout route to correct place", async ()=> {
        wrapper.find("button").trigger("click");
        await flushPromises();
        expect($router.replace).lastCalledWith ({name: "login"})
    })

    it("sets the correct user to logged in", async () => {
        await wrapper.vm.$nextTick();
    
        expect(wrapper.vm.$data.loggedIn).toBe(true);
      });


describe("secret vue", () => {
  let wrapper;
  it("renders", () => {
    wrapper = mount(Secret, { mocks: { $axios } });
    expect(wrapper.exists()).toBe(true);
  });

  it("shows correct name", async () => {
    wrapper = mount(Secret, { mocks: { $axios } });
    await flushPromises();
    const l = wrapper.find("h5");
    expect(l.text()).toBe("123");
  });
});

})