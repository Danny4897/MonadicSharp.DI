import { defineConfig } from 'vitepress'

export default defineConfig({
  title: 'MonadicSharp.DI',
  description: 'Lightweight functional mediator for .NET — CQRS aligned with MonadicSharp primitives. Handlers return Result<T>, not exceptions.',
  base: '/MonadicSharp.DI/',
  cleanUrls: true,
  ignoreDeadLinks: true,

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
          {
            text: 'Core',
            items: [
              { text: 'MonadicSharp', link: 'https://danny4897.github.io/MonadicSharp/' },
              { text: 'MonadicSharp.Framework', link: 'https://danny4897.github.io/MonadicSharp.Framework/' },
            ],
          },
          {
            text: 'Extensions',
            items: [
              { text: 'MonadicSharp.AI', link: 'https://danny4897.github.io/MonadicSharp.AI/' },
              { text: 'MonadicSharp.Recovery', link: 'https://danny4897.github.io/MonadicSharp.Recovery/' },
              { text: 'MonadicSharp.Azure', link: 'https://danny4897.github.io/MonadicSharp.Azure/' },
              { text: 'MonadicSharp.DI', link: 'https://danny4897.github.io/MonadicSharp.DI/' },
            ],
          },
          {
            text: 'Tooling',
            items: [
              { text: 'MonadicLeaf', link: 'https://danny4897.github.io/MonadicLeaf/' },
              { text: 'MonadicSharp × OpenCode', link: 'https://danny4897.github.io/MonadicSharp-OpenCode/' },
              { text: 'AgentScope', link: 'https://danny4897.github.io/AgentScope/' },
            ],
          },
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
        {
          text: 'Ecosystem',
          collapsed: true,
          items: [
            { text: 'MonadicSharp ↗', link: 'https://danny4897.github.io/MonadicSharp/' },
            { text: 'MonadicSharp.Framework ↗', link: 'https://danny4897.github.io/MonadicSharp.Framework/' },
            { text: 'MonadicSharp.AI ↗', link: 'https://danny4897.github.io/MonadicSharp.AI/' },
            { text: 'MonadicSharp.Recovery ↗', link: 'https://danny4897.github.io/MonadicSharp.Recovery/' },
            { text: 'MonadicSharp.Azure ↗', link: 'https://danny4897.github.io/MonadicSharp.Azure/' },
            { text: 'MonadicSharp.DI ↗', link: 'https://danny4897.github.io/MonadicSharp.DI/' },
            { text: 'MonadicLeaf ↗', link: 'https://danny4897.github.io/MonadicLeaf/' },
            { text: 'MonadicSharp × OpenCode ↗', link: 'https://danny4897.github.io/MonadicSharp-OpenCode/' },
            { text: 'AgentScope ↗', link: 'https://danny4897.github.io/AgentScope/' },
          ],
        },
      ],
    },

    socialLinks: [
      { icon: 'github', link: 'https://github.com/Danny4897/MonadicSharp.DI' },
    ],

    search: { provider: 'local' },

    editLink: {
      pattern: 'https://github.com/Danny4897/MonadicSharp.DI/edit/main/docs/:path',
      text: 'Edit this page on GitHub',
    },


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
