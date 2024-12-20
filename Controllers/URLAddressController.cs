﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using URLShortener.Infrastructure;
using URLShortener.Models.URLAddresses;
using URLShortenerData.Data;
using URLShortenerData.Data.Entities;
using System.Linq;

namespace URLShortener.Controllers
{
public class URLAddressController : Controller
{
    private readonly URLShortenerDbContext context;

    public URLAddressController(URLShortenerDbContext data)
    {
        this.context = data;
    }

    public IActionResult All() // Action to display all URL addresses
    {
        List<URLAddressViewModel> urlAddresses = this.context.URLAddresses
            .Select(x => new URLAddressViewModel
            {

                Id = x.Id,
                OriginalUrl = x.OriginalUrl,
                ShortUrl = x.ShortUrl,
                DateCreated = x.DateCreated,
                Visits = x.Visits
            }).ToList();

        // Return the list of URLs with an index (STT)
        return this.View(urlAddresses.Select((url, index) => new URLAddressViewModel
        {
            STT = index + 1,  // Add STT index here (index + 1 because index starts from 0)
            Id = url.Id,
            OriginalUrl = url.OriginalUrl,
            ShortUrl = url.ShortUrl,
            DateCreated = url.DateCreated,
            Visits = url.Visits
        }).ToList());
    }

    [Authorize]
    public IActionResult Add() // GET: Display the form for adding a new URL
    {
        return this.View();
    }

    [Authorize]
    [HttpPost]
    public IActionResult Add(URLAddressFormModel model) // POST: Handle form submission for adding a URL
    {
        if (this.context.URLAddresses.Any(c => c.OriginalUrl == model.URL && c.UserId == this.User.UserId()))
        {
            this.ModelState.AddModelError(nameof(model.URL), "You already have a short URL of that original URL!");
        }

        if (this.context.URLAddresses.Any(c => c.ShortUrl.Contains(model.ShortCode)))
        {
            this.ModelState.AddModelError(nameof(model.ShortCode), $"A short code \"{model.ShortCode}\" already exists!");
        }

        if (!this.ModelState.IsValid)
        {
            return this.View(model);
        }

        URLAddress url = new URLAddress
        {
            OriginalUrl = model.URL,
            ShortUrl = "http://shorturl.co/go/" + model.ShortCode,
            DateCreated = DateTime.UtcNow,
            UserId = this.User.UserId(),
            Visits = 0
        };

        this.context.URLAddresses.Add(url);
        this.context.SaveChanges();

        return this.RedirectToAction("Index", "Home");
    }

    [Authorize]
    public IActionResult Edit(int id)
    {
        if (!this.URLExists(id)) return this.BadRequest();

        if (!this.UserIsCreator(id, this.User.UserId())) return this.Unauthorized();

        URLAddress? url = this.GetURLAddressById(id);
        if (url is null) return this.NotFound();

        string theOriginalShortCode = url.ShortUrl.Substring(url.ShortUrl.LastIndexOf('/') + 1);

        URLAddressFormModel urlFormModel = new URLAddressFormModel()
        {
            URL = url.OriginalUrl,
            ShortCode = theOriginalShortCode
        };

        return this.View(urlFormModel);
    }

    [HttpPost]
    [Authorize]
    public IActionResult Edit(int id, URLAddressFormModel model)
    {
        if (!this.URLExists(id)) return this.BadRequest();

        if (!this.UserIsCreator(id, this.User.UserId())) return this.Unauthorized();

        if (!this.ModelState.IsValid)
        {
            return this.View(model);
        }

        URLAddress? url = this.GetURLAddressById(id);
        if (url is null)
        {
            return this.NotFound();
        }

        url.OriginalUrl = model.URL;
        url.ShortUrl = url.ShortUrl.Substring(0, url.ShortUrl.LastIndexOf('/') + 1) + model.ShortCode;

        this.context.SaveChanges();

        return this.RedirectToAction("Index", "Home");
    }

    [Authorize]
    public IActionResult Delete(int id)
    {
        if (!this.URLExists(id)) return this.BadRequest();

        if (!this.UserIsCreator(id, this.User.UserId())) return this.Unauthorized();

        URLAddress? urlData = this.GetURLAddressById(id);
        if (urlData == null) return this.BadRequest();

        URLAddressViewModel model = new URLAddressViewModel()
        {
            Id = urlData.Id,
            OriginalUrl = urlData.OriginalUrl,
            ShortUrl = urlData.ShortUrl,
            DateCreated = urlData.DateCreated,
            Visits = urlData.Visits
        };

        return this.View(model);
    }

    [HttpPost]
    [Authorize]
    public IActionResult Delete(URLAddressViewModel model)
    {
        if (!this.URLExists(model.Id)) return this.BadRequest();

        if (!this.UserIsCreator(model.Id, this.User.UserId())) return this.Unauthorized();

        URLAddress? urlAddress = this.GetURLAddressById(model.Id);
        if (urlAddress is null) return this.NotFound();

        this.context.URLAddresses.Remove(urlAddress);
        this.context.SaveChanges();

        return this.RedirectToAction("Index", "Home");
    }

    public bool URLExists(int id) => this.context.URLAddresses.Any(h => h.Id == id);

    public bool UserIsCreator(int urlId, string userId)
    {
        URLAddress? urlAddress = this.context.URLAddresses.FirstOrDefault(a => a.Id == urlId);
        return urlAddress?.UserId == userId;
    }

    public URLAddress? GetURLAddressById(int id) => this.context.URLAddresses.Find(id);

    [HttpGet("URLAddressController/RedirectTo/{path:required}", Name = "ShortUrls_RedirectTo")]
    public IActionResult RedirectTo(string path)
    {
        if (path is null) return this.NotFound();

        string normalizeUrl = path.Replace("%2F", "/");

        URLAddress urlOriginalAddres = this.context.URLAddresses
            .FirstOrDefault(a => a.ShortUrl == normalizeUrl)!;
        if (urlOriginalAddres?.OriginalUrl is null)
        {
            return this.NotFound();
        }

        urlOriginalAddres.Visits++;
        this.context.SaveChanges();

        return this.Redirect(urlOriginalAddres.OriginalUrl);
    }
}
}
