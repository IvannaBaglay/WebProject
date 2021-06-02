from rest_framework import viewsets
from rest_framework import permissions
from rest_framework import generics

from .models import User
from .serializer import UserSerializer


class UserViewSet(viewsets.ModelViewSet):
    queryset = User.objects.all().order_by('name')
    serializer_class = UserSerializer
    permission_classes = [permissions.IsAuthenticated]