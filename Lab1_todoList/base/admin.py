from django.contrib import admin
from .models import Task, Contact
# Register your models here.

admin.site.register(Task)

@admin.register(Contact)
class ContactAdmin(admin.ModelAdmin):
    list_display = ("name", "email")