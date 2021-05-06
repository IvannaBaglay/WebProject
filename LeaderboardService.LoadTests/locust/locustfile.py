import random, base64, uuid, json, requests
from locust import HttpUser, task, between, tag

auth_service_url = "http://10.41.4.88:42420/api/Users/"

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
		self.post_score()

	@task(1)
	@tag("post_score")
	def post_score(self):
		score = random.randint(0, 18);
		self.client.post("/api/Score", data=str(score), headers={"Content-Type":"application/json","Authorization":self.token}, verify=False);

	@task(1)
	@tag("get_score")
	def get_score(self):
		self.client.get("/api/Score/" + self.login, headers={"Authorization":self.token}, verify=False);


	@task(3)
	@tag("get_top")
	def get_top(self):
		rank = random.randint(0, 10);
		self.client.get(f"/api/Score/Board/{rank}", headers={"Authorization":self.token}, name = "get_top", verify=False);


	def on_stop(self):
		self.delete()	


	def register(self):
		print(self.basic_auth)
		response = requests.post(auth_service_url + "Add", headers={"Authorization":self.basic_auth()}, verify=False);
		if response.status_code == 200:
			jresponce = response.json()
			self.token = "Bearer " + jresponce["token"]

	def delete(self):
		response = requests.delete(auth_service_url + "Delete/Me", headers={"Authorization":self.token}, verify=False);
