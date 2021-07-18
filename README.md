# faas-scale-zero
<p />
Scaling down to zero OpenFaaS functions.<br />
Only functions with a label of <code>com.openfaas.scale.zero=true</code> are scaled to zero, all others are ignored<br />
<p />
Docker:<br />
https://hub.docker.com/r/mslaga/faas-scale-zero <br />
Environment:<br />
- FAAS_SCALE_ZERO_GATEWAY - Gateway URL starting with http(s):// (default "http://127.0.0.1:8080")<br />
- FAAS_SCALE_ZERO_USER. - Gateway username (default "admin")<br />
- FAAS_SCALE_ZERO_PASSWORD - Gateway password<br />
- FAAS_SCALE_ZERO_INTERVAL. - Time interval(e.g.: 30s, 15m, 1h) (default "30m")<br />
<p />
for <code>fassd</code> you can download available binary and run:<br />
<code>sudo ./faasSzero install -g _http://127.0.0.1:8080_ -p _password_</code>
