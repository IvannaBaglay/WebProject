from rest_framework import viewsets
from rest_framework import permissions

from .migrations. import User

class UserSerializer():
    class Meta:
        model = User
        fields = "__all__"

    def get_userset(self):
        user = self.request.user
        return User.objects.filter(User=user)


