from django.core.mail import send_mail
from Lab1_todoList.celery import app

from .service import send
from .models import Contact

# Create your tests here.

@app.task
def send_spam_email(user_email):
    send(user_email)

@app.task
def send_beat_email():
    for contact in Contact.objects.all():
        send_mail(
            'You subscibe on news',
            'We will send spam every 5 minutes',
            'From the hell',
            [contact.email],
            fail_silently = False
      )