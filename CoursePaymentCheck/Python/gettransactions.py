#!/usr/bin/python3
import re
import hashlib
import time
import smtplib
import datetime
import sys
from dateutil.parser import parse
from fints.client import FinTS3PinTanClient

blz = sys.argv[1]
account = sys.argv[2]
kontonummer = sys.argv[3]
pin = sys.argv[4]
start_date = parse(sys.argv[5])
end_date = parse(sys.argv[6])
https_endpoint = sys.argv[7]
file_name = sys.argv[8]

# Connect to the bank
f = FinTS3PinTanClient(blz, account, pin, https_endpoint)
# Get all acounts
konten = f.get_sepa_accounts()

# Get desired account
index = -1
for konto in konten:
    if konto.accountnumber == kontonummer:
        index = konten.index(konto)

bewegungen = f.get_transactions(konten[index], start_date, end_date)

title_date = "Buchungstag"
title_sender = "Beguenstigter/Zahlungspflichtiger"
title_subject = "Verwendungszweck"
title_amount = "Betrag"

with open(file_name, "a") as myfile:
        myfile.write(title_date + ";"+title_sender+";"+title_subject+";"+title_amount+"\n")

for bewegung in bewegungen:
    date = str(bewegung.data.get('date', ''))
    senderOrReceiver = str(bewegung.data.get('applicant_name', ''))
    subject = str(bewegung.data.get('posting_text', ''))
    amount = str(bewegung.data.get('amount', ''))
    amount = amount.replace("<", "")
    amount = amount.replace(">", "")
    amount = amount.replace("EUR", "")
    amount = amount.rstrip()

    with open(file_name, "a") as myfile:
        myfile.write(date + ";"+senderOrReceiver+";"+subject+";"+amount+"\n")