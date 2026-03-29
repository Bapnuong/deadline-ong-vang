using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class UITest
{
    [Test]

    public void TestActiveUIMenu()
    {
        // Arrange
        GameObject uiMenu = new GameObject("UIMenu");
        uiMenu.SetActive(false);
        // Act
        uiMenu.SetActive(true);
        // Assert
        Assert.IsTrue(uiMenu.activeSelf, "The UI menu should be active.");
    }

    [Test]

    public void TestUIButtonClick()
    {
        // Arrange
        GameObject button = new GameObject("UIButton");
        bool isClicked = false;
        button.AddComponent<UnityEngine.UI.Button>().onClick.AddListener(() => isClicked = true);
        // Act
        button.GetComponent<UnityEngine.UI.Button>().onClick.Invoke();
        // Assert
        Assert.IsTrue(isClicked, "The button should have been clicked.");
    }

    [Test]

    public void TestUIElementPosition()
    {
        // Arrange
        GameObject uiElement = new GameObject("UIElement");
        RectTransform rectTransform = uiElement.AddComponent<RectTransform>();
        Vector3 expectedPosition = new Vector3(100, 100, 0);
        // Act
        rectTransform.anchoredPosition = expectedPosition;
        // Assert
        Assert.AreEqual(expectedPosition, rectTransform.anchoredPosition, "The UI element should be at the expected position.");
    }

    [Test]

    public void TestUIElementVisibility()
    {
        // Arrange
        GameObject uiElement = new GameObject("UIElement");
        // Act
        uiElement.SetActive(false);
        // Assert
        Assert.IsFalse(uiElement.activeSelf, "The UI element should be invisible.");
    }

    [Test]

    public void TestUIElementScale()
    {
        // Arrange
        GameObject uiElement = new GameObject("UIElement");
        RectTransform rectTransform = uiElement.AddComponent<RectTransform>();
        Vector3 expectedScale = new Vector3(2, 2, 1);
        // Act
        rectTransform.localScale = expectedScale;
        // Assert
        Assert.AreEqual(expectedScale, rectTransform.localScale, "The UI element should have the expected scale.");
    }

    [Test]

    public void TestUIElementRotation()
    {
        // Arrange
        GameObject uiElement = new GameObject("UIElement");
        RectTransform rectTransform = uiElement.AddComponent<RectTransform>();
        Quaternion expectedRotation = Quaternion.Euler(0, 0, 45);
        // Act
        rectTransform.rotation = expectedRotation;
        // Assert
        Assert.AreEqual(expectedRotation, rectTransform.rotation, "The UI element should have the expected rotation.");
    }

}
