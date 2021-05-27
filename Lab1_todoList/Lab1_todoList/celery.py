import os 
from celery import Celery

os.environ.setdefault('DJANGO_SETTINGS_MODULE', 'Lab1_todoList.settings.setting')

app = Celery('send_email')
app.config_from_object('django.conf:settings', namespace='CELETY')
app.autodiscover_tasks()

#celery beat task

# app.conf.beat_schedule = {
#   'send-spam-every-10-minutes': {
#       'task': 'main.tasks.send_beat_email',
#        'schedule': crontab(minute='/10'), 
# 
#       },
# 
# 
# }