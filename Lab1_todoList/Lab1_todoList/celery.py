import os 
from celery import Celery
from celery.schedules import crontab

os.environ.setdefault('DJANGO_SETTINGS_MODULE', 'Lab1_todoList.settings')

app = Celery('send_email')
app.config_from_object('django.conf:settings', namespace='CELERY')
app.autodiscover_tasks()

# Load task modules from all registered Django app configs.
app.autodiscover_tasks()

#celery beat task

app.conf.beat_schedule = {
  'send-spam-every-5-minutes': {
      'task': 'base.tasks.send_beat_email',
       'schedule': crontab(minute='*/5'), 

      },


}