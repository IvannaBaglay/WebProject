from django.core.mail import send_mail

def send(user_email):
    send_mail('Spam Message','Text Message', 'From hell', [user_email], fail_silently=False)