const NitroConfig = {
    "config.urls": ['/nitro/renderer-config.json', '/nitro/ui-config.json'],
    "sso.ticket": localStorage.getItem('nitro.sso_ticket'),
    "forward.type": (new URLSearchParams(window.location.search).get('room') ? 2 : -1),
    "forward.id": (new URLSearchParams(window.location.search).get('room') || 0),
    "friend.id": (new URLSearchParams(window.location.search).get('friend') || 0),
};