#!/bin/bash

trap 'kill -SIGINT $PID' SIGTERM
mono /srv/slackbot/SOCVR.Slack.StatBot.exe &
PID=$!
wait $PID
#trap - TERM INT
#wait $PID
#EXIT_STATUS=$?