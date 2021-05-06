import random, base64, uuid, json
from locust import HttpUser, task, between, tag

class User(HttpUser):
	wait_time = between(3, 14)

	login = "locust_user_"
	password = "pass"
	token = ""

	def basic_auth(self):
		return "Basic " + base64.b64encode(str.encode(self.login + ":" + self.password)).decode("ascii")


	def on_start(self):
		self.login += str(uuid.uuid4());
		self.register()

	@task(3)
	@tag("refresh")
	def refresh(self):
		self.client.get("/api/Users/Refresh", headers={"Authorization":self.token}, verify=False);

	@task(5)
	@tag("auth_test")
	def auth_test(self):
		self.client.get("/api/Users/AuthorizeTest", headers={"Authorization":self.token}, verify=False);

	def on_stop(self):
		self.delete()	

	def register(self):
		print(self.basic_auth)
		response = self.client.post("/api/Users/Add", headers={"Authorization":self.basic_auth()}, verify=False, catch_response=True);
		if response.status_code == 200:
			jresponce = json.loads(response.text)
			self.token = "Bearer " + jresponce["token"]

	def delete(self):
		response = self.client.delete("/api/Users/Delete/Me", headers={"Authorization":self.token}, verify=False, catch_response=True);