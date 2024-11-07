// Tasks:
// Never-see is not working on local files
const __IN_DEV__ = false;
const __CAPTURE__EVENT = !__IN_DEV__;
// tested on: (4*(2+1)=12)%
const __TRAFFIC_PORTION__ = 10;

const __LOCAL_STORAGE_KEY__ = "__app_newsletter_v0.0.1__"

const dataUtils = {
    getData: () => {
        const data = localStorage.getItem(__LOCAL_STORAGE_KEY__);
        let ob = {
            first_time: true, never: false,
        }
        if (data) {
            ob = JSON.parse(data);
        }
        return ob;
    },
    setData: (data) => {
        localStorage.setItem(__LOCAL_STORAGE_KEY__, JSON.stringify(data));
    },
    setItSeen: () => {
        dataUtils.setData({
            ...dataUtils.getData(), first_time: false
        })
    },
    setItNever: () => {
        dataUtils.setData({
            ...dataUtils.getData(), never: true
        })
    }
}

const utils = {
    createElementFromHTML: (htmlString) => {
        const div = document.createElement('div');
        div.innerHTML = htmlString.trim();
        return div.firstChild;
    },
    captureEvent: (event, props) => {
        if (__IN_DEV__)
            console.info("capturing event", event, props);
        if (!__CAPTURE__EVENT) return;
        const eventPrefix = __IN_DEV__ ? "test-" : "prod-"
        posthog.capture(eventPrefix + event, props)
    }
}

const htmlString = `
            <div>
            <style>
            .app-newsletter{
                font-family: "DM Sans",sans-serif;

                position: fixed;
                top: 0;
                left: 0;
                right: 0;
                bottom: 0;
                height: 100%;
                z-index: 10001;
                transition-duration: 0.3s;
                transition-property: top,left;
            }
            .app-newsletter.as-modal{
                background-color: rgba(74,74,74,0.26);
            }
            .app-newsletter.as-toast {
                left: calc(100vw - 275px);
                top: calc(100vh - 230px);
                width: 250px;
                height: 250px;
            }
            .app-newsletter.as-toast [data-close]{
                display: none;
            }
            .app-newsletter.as-modal [data-maximize]{
                display: none;
            }
            .video{
               margin-top: 12px;
               /*border-radius: 8px;*/
               width: calc(100% + 32px);
               margin-left: calc(-16px);
               margin-right: calc(-16px);    
               
            }
            .app-newsletter.as-modal .video{
            }
            .app-newsletter.as-toast .video{
                display: none;
            }
            .backdrop{
                position: fixed;
                top: 0;
                left: 0;
                right: 0;
                bottom: 0;
                height: 100%;
                z-index: 10000;
                background-color: rgba(74,74,74,0.26);
            }
            .app-newsletter .wrapper-1{
                display: flex;
                height: 100%;
                align-items: center;
                justify-content: center;
            }
            .app-newsletter .wrapper-2{              
                background: white;
                color: #333e45;
                padding: 16px;
                border-radius: 8px;
                box-shadow: 1px 2px 4px 1px #afafaf5c;        
            }
            [data-action="main"]{
                cursor:pointer;
            }
        </style>
        <div>
           
            <link href="https://fonts.googleapis.com/css2?family=DM+Sans:wght@100;300;400;500;600;700&display=swap" rel="stylesheet">
            <div id="__APP_NEWSLETTER__" class="app-newsletter" data-newsletter>
                    <div class="wrapper-1">
                        <div class="wrapper-2">
                            <div style="display: flex; justify-content: space-between; align-items: center">
                                <div data-action="main" style="font-weight: 600; font-size: 20px; line-height: 1">Flutter Free Pages</div>
                                <svg data-close style="cursor: pointer" xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="lucide lucide-x"><path d="M18 6 6 18"/><path d="m6 6 12 12"/></svg>
                                <svg data-maximize style="cursor: pointer" xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="lucide lucide-maximize-2"><polyline points="15 3 21 3 21 9"/><polyline points="9 21 3 21 3 15"/><line x1="21" x2="14" y1="3" y2="10"/><line x1="3" x2="10" y1="21" y2="14"/></svg>
                            </div>
                            <div style="margin-top: 8px">Ready to use pages for Flutter Multi-Platform</div>
                            <img data-action="main" src="https://getappui.com/app/newsletter/video-dark.gif" class="video">
                            <div style="display: flex; justify-content: center; margin-top: 12px">
                                <div data-action="main"  style="background-color:#703be7; cursor: pointer; color: white; padding: 5px 12px 6px 12px; font-weight: 500; border-radius: 4px;text-decoration: none">Show me</div>
                            </div>
                            <div data-never style="text-align: center; margin-top: 8px; color: #a6a6a6; font-size: 14px;">I don't want to see this again</div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        `

const init = () => {

    if (dataUtils.getData().never) {
        return;
    }

    initHead();

    const body = document.querySelector('body');

    if (!body) {
        return;
    }

    const element = utils.createElementFromHTML(htmlString);
    const closeButton = element.parentElement.querySelector('[data-close]');
    const maximizeButton = element.parentElement.querySelector('[data-maximize]');
    const newsletter = element.parentElement.querySelector('[data-newsletter]');
    const neverButton = element.parentElement.querySelector('[data-never]');
    const mainActions = element.parentElement.querySelectorAll('[data-action=main]');


    const minimize = () => {
        newsletter.classList.add('as-toast');
        newsletter.classList.remove('as-modal');
        utils.captureEvent('event', {click: "minimize"})
    }

    const maximize = () => {
        newsletter.classList.add('as-modal');
        newsletter.classList.remove('as-toast');
        utils.captureEvent('event', {click: "maximize"})
    }

    const neverSee = () => {
        dataUtils.setItNever();
        element.remove()
        utils.captureEvent('event', {click: "never-see"})
    }

    const showNewsletter = () => {
        newsletter.classList.add(dataUtils.getData().first_time ? 'as-modal' : 'as-toast');
        body.append(element);
        if (dataUtils.getData().first_time)
            utils.captureEvent('event', {see: "first-time"})
        else
            utils.captureEvent('event', {see: "repeat"})
        dataUtils.setItSeen();
    }

    const gotoPage = () => {
        window.open("https://getappui.com/pagger",);
        utils.captureEvent('event', {goto: "pagger"})
    }

    // Listener
    closeButton?.addEventListener('click', () => {
        minimize();
    })

    maximizeButton?.addEventListener('click', () => {
        maximize()
    })

    neverButton?.addEventListener('click', () => {
        neverSee();
    })

    mainActions.forEach(e => e.addEventListener('click', () => {
        gotoPage();
    }))

    showNewsletter();
}

const initHead = () => {
    const head = document.getElementsByTagName('head')[0];
    const script = document.createElement('script');
    script.type = 'text/javascript';
    script.text = `!function(t,e){var o,n,p,r;e.__SV||(window.posthog=e,e._i=[],e.init=function(i,s,a){function g(t,e){var o=e.split(".");2==o.length&&(t=t[o[0]],e=o[1]),t[e]=function(){t.push([e].concat(Array.prototype.slice.call(arguments,0)))}}(p=t.createElement("script")).type="text/javascript",p.async=!0,p.src=s.api_host+"/static/array.js",(r=t.getElementsByTagName("script")[0]).parentNode.insertBefore(p,r);var u=e;for(void 0!==a?u=e[a]=[]:a="posthog",u.people=u.people||[],u.toString=function(t){var e="posthog";return"posthog"!==a&&(e+="."+a),t||(e+=" (stub)"),e},u.people.toString=function(){return u.toString(1)+".people (stub)"},o="capture identify alias people.set people.set_once set_config register register_once unregister opt_out_capturing has_opted_out_capturing opt_in_capturing reset isFeatureEnabled onFeatureFlags getFeatureFlag getFeatureFlagPayload reloadFeatureFlags group updateEarlyAccessFeatureEnrollment getEarlyAccessFeatures getActiveMatchingSurveys getSurveys getNextSurveyStep onSessionId".split(" "),n=0;n<o.length;n++)g(u,o[n]);e._i.push([i,s,a])},e.__SV=1)}(document,window.posthog||[]);
    posthog.init('phc_PTV4Vc282DuDrb2CHOTU3G4BvQ3aBj5WOMH4xjKNHdC', {api_host: 'https://us.i.posthog.com', person_profiles: 'identified_only'})`
    head.appendChild(script);
}

// Start when it'd need
window.addEventListener('load', () => {
    if ((new Date().getHours() % __TRAFFIC_PORTION__ === 0) || __IN_DEV__) {
        init();
    }
});
