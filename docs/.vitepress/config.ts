import { defineConfig } from 'vitepress'

export default defineConfig({
  title: 'MonadicSharp.DI',
  description: 'Lightweight functional mediator for .NET — CQRS aligned with MonadicSharp primitives. Handlers return Result<T>, not exceptions.',
  base: '/MonadicSharp.DI/',
  cleanUrls: true,

  head: [
    ['meta', { property: 'og:type', content: 'website' }],
    ['meta', { name: 'twitter:card', content: 'summary' }],
  ],

  themeConfig: {
    logo: '/logo.svg',
    siteTitle: 'MonadicSharp.DI',

    nav: [
      {
        text: 'Guide',
        items: [
          { text: 'Getting Started', link: '/getting-started' },
          { text: 'CQRS Pattern', link: '/cqrs' },
          { text: 'Pipeline Behaviors', link: '/pipeline-behaviors' },
        ],
      },
      {
        text: 'API',
        items: [
          { text: 'IQueryHandler<Q,T>', link: '/api/query-handler' },
          { text: 'ICommandHandler<C,T>', link: '/api/command-handler' },
          { text: 'IPipelineBehavior<R,T>', link: '/api/pipeline-behavior' },
          { text: 'INotification', link: '/api/notification' },
        ],
      },
      {
        text: 'Ecosystem',
        items: [
          { text: 'MonadicSharp Core', link: 'https://danny4897.github.io/MonadicSharp/' },
          { text: 'NuGet', link: 'https://www.nuget.org/packages/MonadicSharp.DI' },
        ],
      },
    ],

    sidebar: {
      '/': [
        {
          text: 'Guide',
          items: [
            { text: 'Getting Started', link: '/getting-started' },
            { text: 'CQRS Pattern', link: '/cqrs' },
            { text: 'Pipeline Behaviors', link: '/pipeline-behaviors' },
          ],
        },
        {
          text: 'API Reference',
          items: [
            { text: 'IQueryHandler<Q,T>', link: '/api/query-handler' },
            { text: 'ICommandHandler<C,T>', link: '/api/command-handler' },
            { text: 'IPipelineBehavior<R,T>', link: '/api/pipeline-behavior' },
            { text: 'INotification', link: '/api/notification' },
          ],
        },
      ],
    },

    socialLinks: [
      { icon: 'github', link: 'https://github.com/Danny4897/MonadicSharp.DI' },
    ],

    search: { provider: 'local' },

    footer: {
      message: 'Released under the MIT License.',
      copyright: 'Copyright © 2024–2026 Danny4897',
    },

    outline: { level: [2, 3], label: 'On this page' },
  },

  markdown: {
    theme: { light: 'github-light', dark: 'one-dark-pro' },
  },
})
