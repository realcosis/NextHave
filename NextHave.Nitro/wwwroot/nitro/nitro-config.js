const NitroConfig = {
    "config.urls": ['/renderer-config.json', '/ui-config.json'],
    "sso.ticket":new URLSearchParams(window.location.search).get('sso'),
    "forward.type": (new URLSearchParams(window.location.search).get('room') ? 2 : -1),
    "forward.id": (new URLSearchParams(window.location.search).get('room') || 0),
    "friend.id": (new URLSearchParams(window.location.search).get('friend') || 0),
};