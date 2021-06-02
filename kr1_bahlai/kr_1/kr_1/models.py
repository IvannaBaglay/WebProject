from django.db import models
from django.db.models.fields import CharField
from django.db.models.fields.related import ForeignKey

class User(models.Model):
    name = models.CharField(max_lenght = 150, default = '')
    def __str__(self):
        return self.name
    class Meta:
        oredering = ['name']