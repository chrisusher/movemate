﻿using ChrisUsher.MoveMate.API.Services.Savings;
using ChrisUsher.MoveMate.Shared.DTOs.Savings;

namespace Services.Tests.Savings;

[TestFixture]
public class SavingsServiceTests : SavingsTestsBase
{
    public SavingsServiceTests()
    {
    }

    [Test]
    public async Task CreateSavingAsync_ReturnsSaving()
    {
        var savingsAccount = await SavingsService.CreateSavingsAccountAsync(ServiceTestsCommon.DefaultAccount.AccountId, new CreateSavingsAccountRequest
        {
            Name = "Test Account",
            InitialBalance = 10000,
            SavingType = SavingType.CurrentAccount
        });

        Assert.That(savingsAccount, Is.Not.Null, "Saings Account was not created");
        Assert.That(savingsAccount.SavingsId, Is.Not.EqualTo(Guid.Empty), "Savings Id was not created");
    }

    [Test]
    public async Task GetSavingsAccountsAsync_ReturnsSavings()
    {
        var savings = await SavingsService.GetSavingsAccountsAsync(ServiceTestsCommon.DefaultAccount.AccountId);

        Assert.That(savings, Is.Not.Null, "Savings were not returned");
        Assert.That(savings, Is.Not.Empty, "Savings were not returned");
    }

    [Test]
    public async Task GetSavingAsync_ReturnsSaving()
    {
        var savings = await SavingsService.GetSavingsAccountsAsync(ServiceTestsCommon.DefaultAccount.AccountId);
        var saving = savings.FirstOrDefault();

        if (savings == null)
        {
            saving = await SavingsService.CreateSavingsAccountAsync(ServiceTestsCommon.DefaultAccount.AccountId, new CreateSavingsAccountRequest
            {
                Name = "Test Account",
                InitialBalance = 10000,
                SavingType = SavingType.NotSet
            });
        }

        var savingId = saving.SavingsId;
        saving = await SavingsService.GetSavingsAccountAsync(ServiceTestsCommon.DefaultAccount.AccountId, savingId);

        Assert.That(saving, Is.Not.Null, "Saving was not returned");
        Assert.That(saving.SavingsId, Is.EqualTo(savingId), "Saving was not returned");
    }

    [Test]
    public async Task UpdateSavingAsync_ReturnsSaving()
    {
        var savings = await SavingsService.GetSavingsAccountsAsync(ServiceTestsCommon.DefaultAccount.AccountId);

        var saving = savings.FirstOrDefault();

        if (saving == null)
        {
            saving = await SavingsService.CreateSavingsAccountAsync(ServiceTestsCommon.DefaultAccount.AccountId, new CreateSavingsAccountRequest
            {
                Name = "Test Account",
                InitialBalance = 10000,
                SavingType = SavingType.NotSet
            });
        }

        var newName = "Another Account";
        saving.Name = newName;

        saving = await SavingsService.UpdateSavingsAccountAsync(ServiceTestsCommon.DefaultAccount.AccountId, saving.SavingsId, saving);

        Assert.That(saving, Is.Not.Null, "Saving was not returned");
        Assert.That(saving.Name, Is.EqualTo(newName), "Saving was not returned");
    }
}
