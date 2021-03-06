from base.forms import ContactForm
from django.urls import path
from .views import ContactView, CustomLoginView, TaskList, TaskDetail, TaskCreate, TaskUpdate, DeleteTask, RegisterPage
from django.contrib.auth.views import LogoutView
from . import views

urlpatterns = [
    path('login/', CustomLoginView.as_view(), name='login'),
    path('logout/', LogoutView.as_view(next_page='login'), name='logout'), 
    path('register/', RegisterPage.as_view(), name = 'register'),

    path('', TaskList.as_view(), name='tasks'),
    path('task/<int:pk>/', TaskDetail.as_view(), name='task'),
    path('task-create/', TaskCreate.as_view(), name='task-create'),
    path('task-update/<int:pk>/', TaskUpdate.as_view(), name='task-update'),
    path('task-delete/<int:pk>/', DeleteTask.as_view(), name='task-delete'),
    path('room/<str:room_name>/', views.room, name='room'),

    path('contact/', ContactView.as_view(), name='contact')
]

